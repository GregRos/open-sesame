module UPathExtensions

open Zio
open Zio.FileSystems
open System

let defaultFs = new PhysicalFileSystem()


type UPath with
    member this.defaultFs() = this.fs defaultFs
    member this.fs(fs: IFileSystem) : FileSystemEntry = fs.GetFileSystemEntry this

    member this.ext: string =
        let ext = this.GetExtensionWithDot()
        ext

    member this.isRoot: bool =
        if not this.IsAbsolute then
            false
        else
            match this.Split() |> List.ofSeq |> List.tail with
            | [ "" ] -> true
            | [ "mnt"; x ] when x.Length = 1 -> true
            | _ -> false

    member this.parent: UPath = this.GetDirectory()

    member this.isIn(parent: UPath) : bool = this.IsInDirectory(parent, false)

    member this.isInDeep(parent: UPath) : bool = this.IsInDirectory(parent, true)

    member this.hasExtOf(exts: string list) : bool =
        let ext = this.ext
        exts |> List.exists (fun e -> e = ext)

    member this.walkUp: list<UPath> =
        seq {
            let mutable current = this

            while not current.isRoot do
                yield current
                current <- current.parent
        }
        |> Seq.toList

type FileSystemEntry with
    member this.isRoot: bool = this.Path.isRoot

    member this.walkUp =
        this.Path.walkUp |> List.map (fun p -> p.fs this.FileSystem :?> DirectoryEntry)

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

    member this.dir =
        match this.asDir with
        | Some dir -> dir
        | None -> this.Parent

    member this.isChildOf(parent: UPath) : bool = this.Path.isIn parent

    member this.isDeepChildOf(parent: UPath) : bool = this.Path.isInDeep parent
