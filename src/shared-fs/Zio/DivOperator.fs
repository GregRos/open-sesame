[<AutoOpen>]
module Shared.Zio.DivOperator

#nowarn "77"

open Zio
open Zio.FileSystems
open Shared

type DivOverload = DivOverload
    with

        static member maybeDiv(a: FileSystemEntry option, b: string) : FileSystemEntry option =
            DivOverload.maybeDiv (a, UPath(b))

        static member maybeDiv(a: FileSystemEntry option, b: UPath) : FileSystemEntry option =
            match a with
            | Some a -> DivOverload.maybeDiv (a, b)
            | None -> None

        static member maybeDiv(a: FileSystemEntry, b: string) : FileSystemEntry option =
            DivOverload.maybeDiv (a, UPath(b))

        static member maybeDiv(a: FileSystemEntry, b: UPath) : FileSystemEntry option =
            let path: UPath = DivOverload.div (a.Path, b)
            a.tryGo path


        static member div(a: UPath, b: string) : UPath =
            let path = DivOverload.div (a, UPath(b))
            path

        static member div(a: UPath, b: UPath) =
            let path = UPath.Combine(a, b)
            path

        static member inline div(a: ^T, b: ^T) : ^R =
            ((^T or ^S): (static member (/): ^T * ^T -> ^R) (a, b))

        static member divMany(origin: FileSystemEntry, tail: string) : FileSystemEntry seq = origin.glob tail

        static member divMany(origin: FileSystemEntry, tail: UPath) : FileSystemEntry seq = origin.glob tail

        // FileSystemEntry left
        static member divMany(origin: FileSystemEntry, tail: string seq) : FileSystemEntry seq =
            tail |> Seq.collect (fun t -> origin.glob t)

        static member divMany(origin: FileSystemEntry, tail: UPath seq) : FileSystemEntry seq =
            tail |> Seq.collect (fun t -> origin.glob t)

        // FileSystemEntry seq left
        static member divMany(origins: #seq<FileSystemEntry>, tail: string) : FileSystemEntry seq =
            origins |> Seq.collect (fun o -> o.glob tail)

        static member divMany(origins: #seq<#FileSystemEntry>, tail: UPath) : FileSystemEntry seq =
            origins |> Seq.collect (fun o -> o.glob tail)

        static member divMany(origins: #seq<#FileSystemEntry>, tail: seq<string>) : FileSystemEntry seq =
            origins |> Seq.collect (fun o -> tail |> Seq.collect (fun t -> o.glob t))

        static member divMany(origins: #seq<#FileSystemEntry>, tail: seq<UPath>) : FileSystemEntry seq =
            origins |> Seq.collect (fun o -> tail |> Seq.collect (fun t -> o.glob t))

        static member inline _divMany(origin: FileSystemEntry, tail: ^X) : FileSystemEntry seq =
            ((^X or DivOverload): (static member divMany: FileSystemEntry * ^X -> FileSystemEntry seq) (origin, tail))

let inline (/) (origin: ^A) (path: ^B) =
    ((^A or DivOverload): (static member div: ^A * ^B -> ^R) (origin, path))

let inline (/?) (origin: ^T) (path: ^S) : ^R =
    ((^T or DivOverload): (static member maybeDiv: ^T * ^S -> ^R) (origin, path))

let inline (/??) (origin: ^T) (tail: ^S) : FileSystemEntry seq =
    ((^T or DivOverload): (static member divMany: ^T * ^S -> FileSystemEntry seq) (origin, tail))

let fs = new PhysicalFileSystem()
let fl = UPath("").entry fs
let fl2 = fl /? "test" /? "a"

let fl3 = fl /?? "test" /?? "."
