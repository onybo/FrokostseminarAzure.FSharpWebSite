module DocumentDatabase

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq
open DocumentDbSample.Core
open Chessie.ErrorHandling

let private databaseId = "OlavsDemoDB"

let private databaseToRecord client id (database:Database option)=
  match database with
  | Some(db) -> ok {client=client; id=id; collectionsLink=db.CollectionsLink; selfLink=db.SelfLink}
  | None -> fail "No database"

let private toDatabaseResult client (db:Database) = function
  | Net.HttpStatusCode.Created -> 
      ok {client=client; id=databaseId; collectionsLink=db.CollectionsLink; selfLink=db.SelfLink}
  | otherCode ->
      fail ("failed to create database: " + otherCode.ToString())
  
let createDatabase (client:DocumentClient) = async{
  let! result = Async.AwaitTask(client.CreateDatabaseAsync (Database( Id = databaseId)))
  return (toDatabaseResult client result.Resource result.StatusCode)        
}

let getDatabase (client:DocumentClient) =
  client.CreateDatabaseQuery().Where(fun db -> db.Id = databaseId).AsEnumerable() 
  |> Seq.tryFind(fun _ -> true) 
  |> databaseToRecord client databaseId

let private getOrCreateDatabase (client:DocumentClient) = async {
    let db = getDatabase client
    return! match db with
            | Ok (db,_) -> async { return ok db }
            | Fail _ ->  createDatabase client
}

let deleteDatabaseWithLogging (client:DocumentClient) databaseLink : Async<Net.HttpStatusCode> = async{
    printfn "Deleting database: %s" databaseLink
    let! result = Async.AwaitTask(client.DeleteDatabaseAsync databaseLink)
    return result.StatusCode
  }

let deleteDatabase client : Async<Net.HttpStatusCode> = async {
  let db = getDatabase client
  return! match db with
          | Fail msg -> async{
                Logger.trace "No database to delete"
                return Net.HttpStatusCode.Gone
              }
          | Ok (db,_) -> async{
              let! result = Async.AwaitTask (client.DeleteDatabaseAsync db.selfLink)
              return result.StatusCode
            }
}