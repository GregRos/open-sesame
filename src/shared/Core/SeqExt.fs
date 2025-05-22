[<AutoOpen>]
module Shared.SeqExt

let cartesian (a: 'a seq) (b: 'b seq) : ('b * 'a) seq =
    seq {
        for x in b do
            for y in a do
                yield (x, y)
    }
