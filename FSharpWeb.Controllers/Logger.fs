module Logger

open System.Diagnostics

let trace message = 
   Trace.Write message  

let traceResult result = 
   Trace.WriteLine(result.ToString())
