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

type CollectionRecord = {
    client: DocumentClient;
    selfLink: string;
    documentsLink: string;
    id: string;
}

type DocumentRecord = { 
  id: string;
  selfLink: string;
  documentsLink: string;
  documents: seq<Person>
}

type DatabaseRecord = {
  client: DocumentClient;
  id: string;
  collectionsLink: string;
  selfLink: string;
}

let emptyDocumentRecord = {id=""; selfLink=""; documentsLink=""; documents=Seq.empty<Person>}

let valueOrDefault = function
  | Ok (x,_) -> x
  | Fail error -> emptyDocumentRecord
