[<AutoOpen>]
module Shared.Zio.FileSystem

#nowarn "77"

open Zio
open Zio.FileSystems
open Shared.Zio.UPath
open System.IO
open Shared.Zio
open Shared.Internal.Win32
open Shared

let (|FileEntry|DirectoryEntry|) (entry: FileSystemEntry) =
    match entry with
    | :? FileEntry as file -> FileEntry file
    | :? DirectoryEntry as dir -> DirectoryEntry dir
    | _ -> failwith "Unknown file system entry type"

let logger = Loggers.CreateLogger "FileSystem"

type FileSystem with
    member this.entry(path: UPath) : FileSystemEntry = path |> string |> this.entry

    member this.entry(path: string) : FileSystemEntry =
        path |> UPath |> this.GetFileSystemEntry

type FileSystemEntry with
    member this.isRoot: bool = this.Path.isRoot

    member this.walkUp =
        this.Path.walkUp
        |> Seq.map (fun p -> p.entry this.FileSystem :?> DirectoryEntry)

    member this.go(path: string) : FileSystemEntry = this.go (UPath path)

    member this.winPath: string = this.Path.winPath

    member this.go(path: UPath) : FileSystemEntry =
        let newPath = UPath.Combine(this.Path, path)
        this.FileSystem.GetFileSystemEntry newPath

    member this.tryGo(path: string) : FileSystemEntry option = this.tryGo (UPath path)

    member this.tryGo(path: UPath) : FileSystemEntry option =
        let newPath = UPath.Combine(this.Path, path)

        match this.FileSystem.TryGetFileSystemEntry newPath with
        | null -> None
        | entry -> Some entry

    member this.tryGoAll(paths: string seq) : FileSystemEntry seq =
        paths |> Seq.map (fun path -> this.tryGo path) |> Seq.choose id

    member this.tryGoAll(paths: UPath seq) : FileSystemEntry seq =
        paths |> Seq.map (fun path -> this.tryGo path) |> Seq.choose id

    member this.ext: string = this.Path.ext

    member this.isFile: bool =
        match this with
        | FileEntry _ -> true
        | _ -> false

    member this.isDir: bool = not this.isFile

    member this.fs = this.FileSystem


    member this.isChildOf(parent: UPath) : bool = this.Path.isDirectChildOf parent

    member this.isDeepChildOf(parent: UPath) : bool = this.Path.isDeepChildOf parent

type FileEntry with
    member this.isHardLinkOf(other: FileEntry) : bool = this.isHardLinkOf other.Path

    member this.isHardLinkOf(path: UPath) : bool =
        match path.tryEntry this.fs with
        | None -> false
        | _ when path = this.Path -> true
        | Some(FileEntry target) -> areHardLinks target.winPath this.winPath
        | _ -> false

    member this.hardLinkTo(path: UPath) =
        match path.tryEntry this.fs with
        | _ when path = this.Path -> failwith $"Tried to hardlink {this.Path} to itself"
        | Some(FileEntry target) when this.isHardLinkOf target.Path ->
            logger.debug $"Already a hardlink: {this.Path} -> {target.Path}"
        | Some(FileEntry target) -> failwith $"Cannot hardlink to existing file: {this.Path} -> {target.Path}"
        | Some(DirectoryEntry _) -> failwith $"Cannot hardlink to directory: {this.Path} -> {path}"
        | None -> createHardLink (this.winPath, path.winPath)
