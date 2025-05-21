[<AutoOpen>]
module Shared.Zio.FileSystem

#nowarn "77"

open Zio
open Zio.FileSystems
open Shared.Zio.UPath
open System.Text.RegularExpressions
open FSharp.Text.RegexProvider
open GlobExpressions

type Disk() =
    inherit PhysicalFileSystem()

type DriveLetterRegex = Regex< @"^(?<DriveLetter>[a-zA-Z]):[\\|/]" >

type FileSystem with
    member this.entry(path: UPath) : FileSystemEntry = path |> string |> this.entry

    member this.entry(path: string) : FileSystemEntry =
        DriveLetterRegex()
            .TypedReplace(
                path,
                fun m ->
                    let driveLetter = m.Groups.["DriveLetter"].Value.ToLowerInvariant()
                    let driveLetter = $"/mnt/{driveLetter}/"
                    driveLetter
            )
        |> UPath
        |> (fun x -> x.ToAbsolute())
        |> this.GetFileSystemEntry

type FileSystemEntry with
    member this.isRoot: bool = this.Path.isRoot

    member this.walkUp =
        this.Path.walkUp
        |> Seq.map (fun p -> p.entry this.FileSystem :?> DirectoryEntry)

    member this.go(path: string) : FileSystemEntry = this.go (UPath path)

    member this.go(path: UPath) : FileSystemEntry =
        let newPath = this.Path / path
        this.FileSystem.GetFileSystemEntry newPath

    member this.tryGo(path: string) : FileSystemEntry option = this.tryGo (UPath path)

    member this.tryGo(path: UPath) : FileSystemEntry option =
        let newPath = this.Path / path

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



let a = UPath().entry (new PhysicalFileSystem())
