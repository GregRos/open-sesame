namespace open_obsidian_fs

open Flurl
open Shared.Flurl


module Say =
    let x = Url "obisidian://"
    let hello name = printfn "Hello %s" name
