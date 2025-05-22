[<AutoOpen>]
module Shared.Zio.FileSystem

#nowarn "77"

open Zio
open Zio.FileSystems
open Shared.Zio.UPath
open System.IO


type Disk() =
    inherit PhysicalFileSystem()

type FileSystem with
    member this.entry(path: UPath) : FileSystemEntry = path |> string |> this.entry

    member this.entry(path: string) : FileSystemEntry =
        path |> UPath |> this.GetFileSystemEntry

type FileSystemEntry with
    member this.isRoot: bool = this.Path.isRoot

    member this.isSymLink: bool =
        match this with
        | :? FileEntry as file -> file.Attributes.HasFlag FileAttributes.ReparsePoint
        | :? DirectoryEntry as dir -> dir.Attributes.HasFlag FileAttributes.ReparsePoint
        | _ -> false

    member this.tryLinkTarget =
        match this with
        | :? FileEntry as file -> File.ResolveLinkTarget(file.Path.winPath, true) |> Some
        | :? DirectoryEntry as dir -> Directory.ResolveLinkTarget(dir.Path.winPath, true) |> Some
        | _ -> None

    member this.walkUp =
        this.Path.walkUp
        |> Seq.map (fun p -> p.entry this.FileSystem :?> DirectoryEntry)

    member this.go(path: string) : FileSystemEntry = this.go (UPath path)

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
        | :? FileEntry -> true
        | _ -> false

    member this.isDir: bool = not this.isFile

    member this.asFile =
        match this with
        | x when x.isFile -> x :?> FileEntry |> Some
        | _ -> None

    member this.asDir =
        match this with
        | x when x.isDir -> x :?> DirectoryEntry |> Some
        | _ -> None

    member this.fs = this.FileSystem

    member this.dir =
        match this.asDir with
        | Some dir -> dir
        | None -> this.Parent

    member this.isChildOf(parent: UPath) : bool = this.Path.isIn parent

    member this.isDeepChildOf(parent: UPath) : bool = this.Path.isInDeep parent
