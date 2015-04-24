module DocumentDatabase

open System
open System.Linq
open Microsoft.Azure.Documents
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents.Linq
open DocumentDbSample.Core
open Chessie.ErrorHandling

let private databaseId = "OlavsDemoDB"

let private toDatabaseResult client (db:Database) = function
  | Net.HttpStatusCode.Created -> 
      ok db
  | otherCode ->
      fail ("failed to create database: " + otherCode.ToString())
  
let createDatabase (client:DocumentClient) = async{
  let! result = Async.AwaitTask(client.CreateDatabaseAsync (Database( Id = databaseId)))
  return (toDatabaseResult client result.Resource result.StatusCode)        
}

let tryCreateDatabase (client:DocumentClient) =
  try
    Async.RunSynchronously (createDatabase client)
  with
  | exn -> fail (sprintf "failed with exception: %A" exn)

let getDatabase (client:DocumentClient) =
  client.CreateDatabaseQuery().Where(fun db -> db.Id = databaseId).AsEnumerable() 
  |> Seq.tryFind(fun _ -> true) 
  |> failIfNone "Database not found"
  
let private getOrCreateDatabase (client:DocumentClient) = async {
    return! match (getDatabase client) with
            | Ok (db, msgs) -> async { return Ok(db, msgs) }
            | Fail _ ->  createDatabase client
}

let deleteDatabase client : Async<Net.HttpStatusCode> = async {
  let db = getDatabase client
  return! match db with
          | Fail _ -> async{
                Logger.trace "No database to delete"
                return Net.HttpStatusCode.Gone
              }
          | Ok (db, _) -> async{
              let! result = Async.AwaitTask (client.DeleteDatabaseAsync db.SelfLink)
              return result.StatusCode
            }
}