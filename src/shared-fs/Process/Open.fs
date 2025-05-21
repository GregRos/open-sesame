[<AutoOpen>]
module Shared.Process.Open

open System
open System.IO
open System.Runtime.InteropServices
open System.Diagnostics

type CreateProcess = { path: string; args: string }

type CreateProcess with
    member this.Start() =
        let startInfo =
            ProcessStartInfo(
                FileName = this.path,
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = this.args
            )

        let proc = new Process(StartInfo = startInfo)

        proc.Start() |> ignore
        proc
