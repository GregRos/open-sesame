module OpenObsidian.Folders

open Flurl
open Zio
open Shared.Zio
open Shared

type SpecialDir =
    | Obsidian of path: UPath
    | Git of path: UPath


[<EntryPoint>]
let main argv =
    let fs = new Disk()
    printfn "Hello from F#"

    let scanSpecialDirsFrom (file: FileSystemEntry) =
        printfn "scanSpecialDirsFrom: %A" file
        file.walkUp /?? [ ".git"; ".obsidian" ]

    let dirs = scanSpecialDirsFrom (@"C:\codegr\parjs.wiki\parsers\int.md" |> fs.entry)
    printfn "dirs: %A" dirs
    0
