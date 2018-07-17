module Denomination

open System.Globalization

type Denomination = 
    | Orthodox
    | Catholic
    
let private regionOfCulture (culture : CultureInfo) = new RegionInfo(culture.LCID)

let denominationOfRegion (region : RegionInfo) : Denomination = 
    match region.TwoLetterISORegionName.ToUpper() with
    | "AM" | "BY" | "BG" | "CY" | "ER" | "ET" | "GE" | "GR" 
    | "MK" | "MD" | "ME" | "RO" | "RU" | "RS" | "UA" // largest faith group is orthodox

    | "AL" | "BA" | "EE" | "KZ" | "KG" | "LV" | "EG" | "LB"
    | "LT" | "SY" | "UZ" // easter is orthodox
        -> Orthodox

    | _ -> Catholic

let denominationOfCulture (culture : CultureInfo) : Denomination =
    regionOfCulture culture |> denominationOfRegion

let current = denominationOfRegion RegionInfo.CurrentRegion