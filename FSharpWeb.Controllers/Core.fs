module DocumentDbSample.Core

open Microsoft.Azure.Documents.Client
open Chessie.ErrorHandling

let toSecureString (s:string) =
  let sString = new System.Security.SecureString()
  s |> Seq.iter (fun cl -> sString.AppendChar cl)
  sString

[<CLIMutable>]
type Person = {
    name : string
    born : int 
    id : string 
}

type DocumentRecord = {
  id: string;
  selfLink: string;
  documentsLink: string;
  documents: seq<Person>
}




