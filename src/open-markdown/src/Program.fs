module OpenSesame.Folders

open Flurl
open Zio

open Shared.Zio
open Shared
open System
open System.Windows.Input
open System.IO

let (|KeyDown|_|) (key: Key) (_: obj) =
    let isKeyDown = Keyboard.IsKeyDown key
    if isKeyDown then Some() else None

let getOpenMode (file: UPath) =
    let file = file.entry fs

    match file with
    | KeyDown Key.LeftCtrl -> NewTab
    | KeyDown Key.LeftShift -> NewWindow
    | :? FileEntry as file when file.Length = 0 -> NewWindow
    | :? FileEntry -> Default
    | _ -> Default

let docsVault = UPath "C:\\codegr\\__docs.vault"



[<STAThread>]
[<EntryPoint>]
let main argv =
    let doOpen (path: UPath) =
        let cmd = OpenFileCommand.Create(path, getOpenMode path)
        let command = cmd.ProduceCommand()
        let prc = command.Create()
        prc.Start() |> ignore

    let doThing (argv: string array) =
        printfn $"""Arguments: {argv.Length} {argv |> String.concat ", "}"""
        let splat = argv |> String.concat " "
        let path = UPath splat

        doOpen path
        0

    doThing argv
