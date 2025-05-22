[<AutoOpen>]
module OpenSesame.OpenFile

open Shared.Zio
open Shared.Flurl
open Zio
open Flurl
open Shared.Process
open Shared.Logging

let logger = Loggers.CreateLogger "OpenFile"

type OpenObsidianUrl =
    | OpenVault of vault: UPath
    | OpenFileInVault of vault: UPath * file: UPath

    member this.Url =
        match this with
        | OpenVault vault ->
            let url =
                Url("obsidian://adv-uri").query
                    {| vault = vault.GetName()
                       openmode = true |}

            logger.debug $"OpenVault: {url}"
            url
        | OpenFileInVault(vault, file) ->
            let url =
                let x = OpenVault(vault).Url.query {| filepath = file |> fun x -> x.winPath |}

                x

            logger.debug $"OpenFileInVault: {url}"
            url

type OpenDefaultMarkdown =
    | OpenDefaultMarkdown of UPath

    member this.Path =
        this
        |> function
            | OpenDefaultMarkdown x -> x


type OpenFileCommand =
    private
    | OpenObsidian of OpenObsidianUrl
    | OpenFile of OpenDefaultMarkdown

    static member Create(target: UPath) =
        let target = target.norm

        if not target.IsAbsolute then
            failwith $"Path must be absolute. Got {target}"

        match target.tryEntry fs with
        | None -> failwith $"Path not found: {target}"
        | Some(:? FileEntry) -> ()
        | Some(:? DirectoryEntry) -> failwith $"Path is a directory: {target}"
        | Some _ -> failwith $"Path is not a file: {target}"


        let obsidianParentVault = SpecialDir.findObsidianVault target

        match target.ext with
        | ".md"
        | ".markdown" ->
            logger.debug $"Markdown file: {target}"

            match obsidianParentVault with
            | Some vault ->
                let filePath = target.relativeTo vault.Path
                logger.trace $"Relatived path {vault.Path} -> {filePath}"

                let openObsidian = OpenFileInVault(vault.Path, filePath) |> OpenObsidian
                openObsidian
            | None ->
                logger.trace $"No obsidian vault found for {target}, using default."
                OpenDefaultMarkdown target |> OpenFile
        | _ -> failwith $"Only md/markdown supported. Got ${target}"

    member this.ProduceCommand() =
        match this with
        | OpenObsidian url ->
            { CreateProcess.path = url.Url.ToString()
              args = "" }
        | OpenFile markdown ->
            { CreateProcess.path = "typora"
              args = $"\"{markdown.Path.winPath}\"" }
