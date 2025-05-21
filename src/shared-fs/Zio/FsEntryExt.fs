[<AutoOpen>]
module Shared.Zio.FileSystem

#nowarn "77"

open Zio
open Zio.FileSystems
open Microsoft.Extensions.FileSystemGlobbing
open Shared.Zio.UPath
open System.Text.RegularExpressions
open FSharp.Text.RegexProvider

type Disk() =
    inherit PhysicalFileSystem()

type DriveLetterRegex = Regex< @"^(?<DriveLetter>[a-zA-Z]):\\" >

type FileSystem with
    member this.entry(path: UPath) : FileSystemEntry = this.GetFileSystemEntry path

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
        |> this.entry

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

    member this.glob(patterns: UPath seq) : seq<FileSystemEntry> =
        patterns |> Seq.map (fun p -> p |> string |> this.glob) |> Seq.concat

    member this.glob(path: UPath) : seq<FileSystemEntry> = this.glob (path.ToString())

    member this.glob(pattern: string) : seq<FileSystemEntry> =
        let matcher = Matcher()
        let matcher = matcher.AddInclude pattern
        let paths = matcher.GetResultsInFullPath(this.Path.ToString())

        paths |> Seq.map UPath |> Seq.map (fun x -> x.entry this.FileSystem)

    member this.glob(patterns: string list) : seq<FileSystemEntry> =
        let matcher = Matcher()

        let included =
            patterns
            |> List.filter (fun p -> not (p.StartsWith "!"))
            |> List.map (fun p -> p.TrimStart('!'))

        matcher.AddIncludePatterns included

        let excluded =
            patterns
            |> List.filter (fun p -> p.StartsWith "!")
            |> List.map (fun p -> p.TrimStart('!'))

        matcher.AddExcludePatterns excluded
        let paths = matcher.GetResultsInFullPath(this.Path.ToString())

        paths |> Seq.map UPath |> Seq.map (fun x -> x.entry this.FileSystem)

let a = UPath().entry (new PhysicalFileSystem())
