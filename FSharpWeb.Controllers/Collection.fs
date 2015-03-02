module DocumentCollection

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq
open DocumentDbSample.Core
open Chessie.ErrorHandling

let private createCollection_ (client:DocumentClient) collectionsLink collectionId = async{
  let! response = Async.AwaitTask(client.CreateDocumentCollectionAsync(collectionsLink, new DocumentCollection( Id = collectionId )))
  return match response.StatusCode with
         | Net.HttpStatusCode.Created -> 
           ok response.Resource 
         | _ ->
           printfn "failed to create collection: %s" (response.StatusCode.ToString()) 
           fail "failed to create collection"
}

let private getOrCreateCollection_ (client:DocumentClient) (db:Database) collectionId = async {
  let collection = client.CreateDocumentCollectionQuery(db.SelfLink).Where(fun c -> c.Id = collectionId).AsEnumerable() |> Seq.tryFind(fun _ -> true)
  return! match collection with
          | None -> createCollection_ client db.CollectionsLink collectionId
          | Some c -> async { return ok c }
}

let getOrCreateCollectionSync (client:DocumentClient) (db:Database) =
    getOrCreateCollection_ client db "Persons" |> Async.RunSynchronously 

let getFirstCollection (client:DocumentClient) (db:Database) =
  client.CreateDocumentCollectionQuery(db.SelfLink).AsEnumerable() 
  |> Seq.tryFind(fun _ -> true) 
  |> failIfNone "no collection created"
  