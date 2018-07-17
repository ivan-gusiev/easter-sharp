module Easter

open System
open Denomination

let private dateFromDE year d e denomination = 
    let f : float = float (d + e)
    match denomination with
        | Catholic -> if (d, e) = (29, 6) then new DateTime(year, 4, 19) else
                      if (d, e) = (28, 6) then new DateTime(year, 4, 18) else
                        (new DateTime(year, 3, 22)).AddDays(f)
        | Orthodox -> (new DateTime(year, 4, 4)).AddDays(f)

let gaussAlgorithm year denomination =
    let a = year % 19
    let b = year % 4
    let c = year % 7
    let k = year / 100
    let p = (13 + 8 * k) / 25
    let q = k / 4
    let (M, N) = match denomination with
                    | Catholic -> ((15 - p + k - q) % 30, (4 + k - q) % 7)
                    | Orthodox -> (15, 6)
    let d = (19 * a + M) % 30
    let e = (2 * b + 4 * c + 6 * d + N) % 7

    dateFromDE year d e denomination
