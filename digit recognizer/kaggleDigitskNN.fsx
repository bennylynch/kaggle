open System
open System.IO
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

type ImageInstance = { Number:int; Pixels:int[] }

let parseTrainData path = File.ReadAllLines(path) |> Array.map (fun line -> line.Split(','))
                       |> Seq.skip 1 |> Array.ofSeq
                       |> Array.map (fun arr -> arr |> Array.map int)
                       |> Array.map (fun arr -> {Number = arr.[0]; Pixels = arr.[ 1 ..]})

let parseTestData path  = File.ReadAllLines(path) |> Array.map (fun line -> line.Split(','))
                              |> Seq.skip 1 
                              |> Array.ofSeq
                              |> Array.map (fun arr -> arr |> Array.map int)

let trainingData = parseTrainData (__SOURCE_DIRECTORY__ + @"\train.csv")

let pointDiff ex1 ex2 = Array.map2(fun x1 x2 -> float (x1 - x2) ** 2.0) ex1 ex2
                        |> Array.sum 
                        |> sqrt

let classByk (unknown: int[]) k = let top1 = trainingData
                                             |> Array.map ( fun ex -> ex.Number, pointDiff ex.Pixels unknown )
                                             |> Array.sortBy (fun (ex,diff) -> diff)
                                             |> Seq.take k
                                             |> Seq.groupBy ( fun (n,diff) -> n)
                                             |> Seq.maxBy ( fun (ky,sq) -> (sq |> Array.ofSeq).Length)
                                  fst top1
let unknowns = parseTestData (__SOURCE_DIRECTORY__ + @"\test.csv")

let predicted = unknowns |> Array.Parallel.map(fun ex -> classByk ex 20)
File.WriteAllLines((__SOURCE_DIRECTORY__ + @"\predicted.txt"),(predicted |> Array.map string))