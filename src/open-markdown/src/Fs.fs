[<AutoOpen>]
module OpenSesame.Fs

open Zio.FileSystems

let fs = new PhysicalFileSystem()
