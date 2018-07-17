module Main

open System
open Denomination
open Easter

[<EntryPoint>]
let main argv =
    let denomination = Denomination.current
    Console.WriteLine ("Current denomination determined automatically as {0}", denomination)

    let year = DateTime.UtcNow.Year
    Console.WriteLine ("Year determined automatically as {0}", year)

    for year in [2001; 2004; 2007; 2010; 2011; 2017; 2025] do
        Console.WriteLine (gaussAlgorithm year Catholic = gaussAlgorithm year Orthodox)

    0 // return an integer exit code