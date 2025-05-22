module OpenSesame.Folders

open Flurl
open Zio

open Shared.Zio
open Shared
open Fake.IO
open Fake.IO.FileSystemOperators
open System

[<EntryPoint>]
let main argv =
    let doThing (argv: string array) =
        printfn $"""Arguments: {argv.Length} {argv |> String.concat ", "}"""
        let splat = argv |> String.concat " "
        let path = UPath splat
        let cmd = OpenFileCommand.Create path
        let command = cmd.ProduceCommand()
        let prc = command.Create()
        prc.Start() |> ignore
        0

    doThing [| @"C:\codegr\parjs.wiki\parsers\int.md" |]
