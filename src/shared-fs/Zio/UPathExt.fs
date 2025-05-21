[<AutoOpen>]
module Shared.Zio.UPath

#nowarn "77"
open Zio



type UPath with

    member this.tryEntry(fs: IFileSystem) : FileSystemEntry option =
        match fs.TryGetFileSystemEntry this with
        | null -> None
        | entry -> Some entry

    member this.entry(fs: IFileSystem) : FileSystemEntry = fs.GetFileSystemEntry this

    member this.ext: string =
        let ext = this.GetExtensionWithDot()
        ext

    member this.isRoot: bool =
        if not this.IsAbsolute then
            false
        else
            match this.Split() |> List.ofSeq with
            | [] -> true
            | [ "mnt"; x ] when x.Length = 1 -> true
            | _ -> false

    member this.parent: UPath = this.GetDirectory()

    member this.isIn(parent: UPath) : bool = this.IsInDirectory(parent, false)

    member this.isInDeep(parent: UPath) : bool = this.IsInDirectory(parent, true)

    member this.hasExtOf(exts: string list) : bool =
        let ext = this.ext
        exts |> List.exists (fun e -> e = ext)


    member this.walkUp: seq<UPath> =
        seq {
            let mutable current = this.parent

            while not (current.isRoot && current.IsEmpty) do
                yield current
                current <- current.parent
        }
