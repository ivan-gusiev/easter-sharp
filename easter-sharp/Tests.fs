module Tests

open Easter
open Denomination
open Microsoft.VisualStudio.TestTools.UnitTesting
open FsUnit.MsTest

[<TestMethod>]
let MatchingEasterDays () =
    for year in [2001; 2004; 2007; 2010; 2011; 2017; 2025] do
        gaussAlgorithm year Catholic |> should equal (gaussAlgorithm year Orthodox)