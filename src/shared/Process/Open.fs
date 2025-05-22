[<AutoOpen>]
module Shared.Process.Open

open System
open System.IO
open System.Runtime.InteropServices
open System.Diagnostics
open Shared.Logging

let logger = Loggers.CreateLogger "OpenSesame.Process"
type CreateProcess = { path: string; args: string }

type CreateProcess with
    member this.Create() =
        logger.debug $"Starting process: {this.path} {this.args}"

        let startInfo =
            ProcessStartInfo(FileName = this.path, UseShellExecute = true, Arguments = this.args)

        let proc = new Process(StartInfo = startInfo)

        proc

    member this.Start() =
        try
            let proc = this.Create()
            proc.Start() |> ignore
            logger.debug $"Started process: {this.path} {this.args}"
            Some proc
        with :? System.ComponentModel.Win32Exception as ex ->
            logger.error $"Failed to start process: {this.path} {this.args} - {ex.Message}"
            None
