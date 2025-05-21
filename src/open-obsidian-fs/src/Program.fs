module OpenObsidian.Folders

open Flurl
open Zio

open Shared.Zio
open Shared
open Fake.IO
open Fake.IO.FileSystemOperators

type SpecialDir =
    | Obsidian of entry: FileSystemEntry
    | Git of entry: FileSystemEntry

[<EntryPoint>]
let main argv =
    let fs = new Disk()
    printfn "Hello from F#"

    let checkForSpecialDirs (file: FileSystemEntry) =
        printfn "scanSpecialDirsFrom: %A" file

        match [ ".obsidian"; ".git" ] |> List.map (fun subpath -> file.tryGo subpath) with
        | Some obsidian :: _ -> Some(Obsidian obsidian)
        | _ :: Some git :: [] -> Some(Git git)
        | _ -> None

    let paths =
        UPath(@"C:\codegr\parjs.wiki\parsers\int.md").walkUp
        |> Seq.map fs.entry
        |> Seq.map (fun x ->
            printfn "walkUp: %A" x
            x)
        |> Seq.map checkForSpecialDirs
        |> Seq.tryPick id

    match paths with
    | Some dir -> printfn "Found special dir: %A" dir
    | None -> printfn "No special dir found"

    0
