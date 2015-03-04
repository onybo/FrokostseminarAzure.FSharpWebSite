module Logger

open DocumentDbSample.Core
open System.Diagnostics

let trace message = 
   Trace.Write message  

let trace2 message category =
  Trace.Write(message, category)