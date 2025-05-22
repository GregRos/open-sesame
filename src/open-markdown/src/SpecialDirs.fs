[<AutoOpen>]
module OpenSesame.SpecialDirs

open Shared.Zio
open Zio
open Zio.FileSystems
open Shared.Logging

let logger = Loggers.CreateLogger "OpenSesame.SpecialDirs"

type SpecialDir =
    | Obsidian of entry: FileSystemEntry
    | Git of entry: FileSystemEntry


    static member private checkHere(entry: FileSystemEntry) =
        logger.debug $"Checking {entry.Path} and up for special dirs"
        let specials = [ ".obsidian", Obsidian; ".git", Git ]

        let findSpecial (special: string, ctor: FileSystemEntry -> SpecialDir) =
            match entry.tryGo special with
            | Some entry -> Some(ctor entry)
            | None -> None

        let found = specials |> List.tryPick findSpecial
        found

    static member findObsidianVault(path: UPath) =
        logger.debug $"Finding Obsidian vault for {path}"

        let paths =
            path.walkUp
            |> Seq.map fs.entry
            |> Seq.map (fun x ->
                printfn "walkUp: %A" x
                x)
            |> Seq.map SpecialDir.checkHere
            |> Seq.filter (function
                | Some(Obsidian _ | Git _) -> true
                | _ -> false)
            |> Seq.tryPick id
            |> Option.bind (function
                | Obsidian entry -> Some entry.Parent
                | Git entry -> None)

        match paths with
        | Some dir -> logger.debug $"Found special dir: {dir}"
        | None -> logger.debug $"No special dir found"

        paths
