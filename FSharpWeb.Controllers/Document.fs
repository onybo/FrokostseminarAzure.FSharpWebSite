module Document

open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq

open DocumentDbSample.Core 
open Chessie.ErrorHandling

let createDocument link doc (client:DocumentClient) = async {
  return! Async.AwaitTask (client.CreateDocumentAsync(link, doc))
}

let private createPersonQuery documentsLink (client:DocumentClient) =
    client.CreateDocumentQuery<Person>(documentsLink).ToList()

let getDocuments (client:DocumentClient) (collection:DocumentCollection) =
  let documents = createPersonQuery collection.DocumentsLink client  //collection.client.CreateDocumentQuery<Person>(collection.documentsLink).ToList()
  ok documents
  
let getPersons documentsLink (client:DocumentClient) =
    query { 
        for person in client.CreateDocumentQuery<Person>(documentsLink) do
        select person 
    } |> Seq.toList