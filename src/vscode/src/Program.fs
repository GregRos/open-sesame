open System
open System.Diagnostics
open Shared.Process
open Shared.Logging

module Vscode =
    let logger = Loggers.CreateLogger "OpenSesame.VSCode"

    [<EntryPoint>]
    let main args =
        let jArgs = args |> String.concat " "
        let names = [ "code-insiders"; "code" ]

        let success =
            names
            |> List.map (fun name ->
                { CreateProcess.path = name
                  args = jArgs })
            |> List.map (fun x -> x.Start())
            |> List.tryPick id
            |> Option.isSome

        if not success then
            logger.warn "Failed to open VSCode"
            Environment.Exit(1)

        0
