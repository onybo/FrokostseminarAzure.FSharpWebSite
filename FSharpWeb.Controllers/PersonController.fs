module CollectionController

open System
open System.Web.Http
open Document
open DocumentDbSample.Core
open Microsoft.Azure.Documents.Client

[<RoutePrefix("api/Person")>]
type CollectionController() =
  inherit ApiController()
  
  let getDbClient = 
    let uri = Uri "https://olavsstorage.documents.azure.com:443/"
    let authKey = toSecureString (System.Configuration.ConfigurationManager.AppSettings.Get("AuthKey"))
    new DocumentClient (uri, authKey=authKey)

  [<Route("")>]
  member x.GetPerson(documentsLink) = 
    use client = getDbClient
    client
    |> getPersons documentsLink