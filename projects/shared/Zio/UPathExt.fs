[<AutoOpen>]
module Shared.Zio.UPath

#nowarn "77"

open Zio
open System.IO
open System.Text.RegularExpressions
open FSharp.Text.RegexProvider

type DriveLetterRegex = Regex< @"^(?<DriveLetter>[a-zA-Z]):[\\|/]" >
type MntRootRegex = Regex< @"^/mnt/(?<DriveLetter>[a-zA-Z])/" >

type UPath with

    member this.tryEntry(fs: IFileSystem) : FileSystemEntry option =
        match fs.TryGetFileSystemEntry this.norm with
        | null -> None
        | entry -> Some entry

    member this.entry(fs: IFileSystem) : FileSystemEntry = fs.GetFileSystemEntry this.norm

    member this.name = this.GetName()

    member this.norm =
        DriveLetterRegex()
            .TypedReplace(
                this.ToString(),
                fun m ->
                    let driveLetter = m.Groups.["DriveLetter"].Value.ToLowerInvariant()
                    let driveLetter = $"/mnt/{driveLetter}/"
                    driveLetter
            )
        |> UPath

    member this.ext: string =
        let ext = this.GetExtensionWithDot()
        ext

    member this.relativeTo(other: UPath) =
        let myPath = this.winPath
        let otherPath = other.winPath
        let result = Path.GetRelativePath(otherPath, myPath)
        UPath result |> fun x -> x.norm

    member this.isRoot: bool =
        if not this.IsAbsolute then
            false
        else
            match this.Split() |> List.ofSeq with
            | [] -> true
            | [ "mnt"; x ] when x.Length = 1 -> true
            | _ -> false

    member this.parent: UPath =
        if this.isRoot || this.IsEmpty then
            this
        else
            this.GetDirectory()

    member this.isDirectChildOf(parent: UPath) : bool = this.IsInDirectory(parent, false)

    member this.isDeepChildOf(parent: UPath) : bool = this.IsInDirectory(parent, true)


    member this.winPath: string =
        MntRootRegex()
            .TypedReplace(
                this.ToString(),
                fun m ->
                    let driveLetter = m.Groups.["DriveLetter"].Value.ToUpperInvariant()
                    let driveLetter = $"{driveLetter}:\\"
                    driveLetter
            )
            .Replace("/", "\\")

    member this.walkUp: seq<UPath> =
        seq {
            let mutable current = this.parent

            while not (current.isRoot || current.IsEmpty) do
                yield current
                current <- current.parent
        }
