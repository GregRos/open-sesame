module Shared.Flurl

open Flurl

type Flurl.Url with
    member this.path() : string = this.Path.ToString()

    member this.path(x: string) : Flurl.Url = this.Clone().AppendPathSegment(x)

    member this.path(xs: string list) : Flurl.Url =
        this.Clone().AppendPathSegments(xs |> List.map box |> List.toArray)

    member this.query(x: string) : Flurl.Url =
        let clone = this.Clone()
        clone.Query <- x
        clone

    member this.query(xs: (string * string) list) : Flurl.Url =
        let clone = this.Clone()
        xs |> List.iter (fun (k, v) -> clone.SetQueryParam(k, v) |> ignore)
        clone

    member this.query(x: obj) : Flurl.Url = this.Clone().SetQueryParams(x)

    member this.host(x: string) : Flurl.Url =
        let clone = this.Clone()
        clone.Host <- x
        clone
