module DocumentDbSample.Core

open Microsoft.Azure.Documents.Client

/// create a Success with no messages
let succeed x =
    Choice1Of2 x

/// create a Success with a message
let succeedWithMsg x msg =
    Choice1Of2 (x,[msg])

/// create a Failure with a message
let fail msg =
    Choice2Of2 [msg]

/// given a Result, map the messages to a different error type
let mapMessages f result = 
    match result with 
    | Choice1Of2 (x,msgs) -> 
        let msgs' = List.map f msgs
        Choice1Of2 (x, msgs')
    | Choice2Of2 errors -> 
        let errors' = List.map f errors 
        Choice2Of2 errors' 

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

//let emptyDatabase client id = {client=client; id=id; collectionsLink=""; selfLink=""}

let valueOrDefault = function
  | Choice1Of2 x -> x
  | Choice2Of2 error -> emptyDocumentRecord
