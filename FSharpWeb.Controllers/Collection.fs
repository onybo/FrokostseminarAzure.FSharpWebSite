module DocumentCollection

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq
open DocumentDbSample.Core
open Chessie.ErrorHandling

let private emptyCollection client = {client=client; selfLink=""; documentsLink=""; id=""}

let private createCollection_ (client:DocumentClient) collectionsLink collectionId = async{
  let! response = Async.AwaitTask(client.CreateDocumentCollectionAsync(collectionsLink, new DocumentCollection( Id = collectionId )))
  return match response.StatusCode with
         | Net.HttpStatusCode.Created -> 
           let x = response.Resource 
           {client=client; selfLink=x.SelfLink; documentsLink=x.DocumentsLink; id=x.Id}
         | _ ->
           printfn "failed to create collection: %s" (response.StatusCode.ToString()) 
           emptyCollection client
}

let private getOrCreateCollection_ (db:DatabaseRecord) collectionId = async {
  let collection = db.client.CreateDocumentCollectionQuery(db.selfLink).Where(fun c -> c.Id = collectionId).AsEnumerable() |> Seq.tryFind(fun _ -> true)
  return! match collection with
          | None -> createCollection_ db.client db.collectionsLink collectionId
          | Some c -> async { return {client=db.client; selfLink=c.SelfLink; documentsLink=c.DocumentsLink; id=c.Id} }
}

let getOrCreateCollectionSync db =
    getOrCreateCollection_ db "Persons" |> Async.RunSynchronously |> ok

let getFirstCollection (db:DatabaseRecord) =

  let client = db.client;
  let collection = client.CreateDocumentCollectionQuery(db.selfLink).AsEnumerable() |> Seq.tryFind(fun _ -> true)
  match collection with
    | None -> fail "no collection created"
    | Some(c) -> ok ({client=client; selfLink=c.SelfLink; documentsLink=c.DocumentsLink; id=c.Id})