module Shared.String

open System

let split (s: string) (delim: char) =
    s.Split(delim)
    |> Array.toList
    |> List.map (fun x -> x.Trim())
    |> List.filter (fun x -> x <> "")

let join (delim: string) (s: string list) =
    let s = s |> List.map (fun x -> x.Trim()) |> List.filter (fun x -> x <> "")
    String.Join(delim, s)
