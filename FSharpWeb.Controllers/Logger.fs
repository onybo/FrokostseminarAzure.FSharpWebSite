module Logger

open DocumentDbSample.Core
open System.Diagnostics

let trace message = 
   Trace.Write message  

let trace2 message category =
  Trace.Write(message, category)

let traceRecord (documentRecord:DocumentRecord) = 
    Trace.Write (sprintf "documentRecord is %A" documentRecord)