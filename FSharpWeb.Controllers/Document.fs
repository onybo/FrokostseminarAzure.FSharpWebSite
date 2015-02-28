module Document

open System.Linq
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq

open DocumentDbSample.Core 

let createDocument link doc (client:DocumentClient) = async {
  return! Async.AwaitTask (client.CreateDocumentAsync(link, doc))
}

let private createPersonQuery documentsLink (client:DocumentClient) =
    client.CreateDocumentQuery<Person>(documentsLink).ToList()

let getDocuments (collection:CollectionRecord) =
  let documents = createPersonQuery collection.documentsLink collection.client  //collection.client.CreateDocumentQuery<Person>(collection.documentsLink).ToList()
  succeed {id=collection.id; selfLink=collection.selfLink; documentsLink=collection.documentsLink; documents = documents;}
  
let getPersons documentsLink (client:DocumentClient) =
    query { 
        for person in client.CreateDocumentQuery<Person>(documentsLink) do
        select person 
    } |> Seq.toList