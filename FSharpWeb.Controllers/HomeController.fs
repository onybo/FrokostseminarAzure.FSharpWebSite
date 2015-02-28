namespace FSharpWeb.Controllers

open System
open System.Web
open System.Web.Routing
open System.Web.Mvc
open EkonBenefits.FSharp.Dynamic
open DocumentDatabase
open DocumentCollection
open Document
open FSharpx.Choice
open DocumentDbSample.Core
open Microsoft.Azure.Documents.Client

type HomeController() =
  inherit Controller()

  let getDbClient = 
    let uri = Uri "https://olavsstorage.documents.azure.com:443/"
    let authKey = toSecureString (System.Configuration.ConfigurationManager.AppSettings.Get("AuthKey"))
    new DocumentClient (uri, authKey=authKey)

  member private x.toViewResult (documents:DocumentRecord) =
    x.View(documents)

  [<HttpGet>]
  [<Route("")>]
  member x.Index() =
    x.ViewBag?Title <-"F# Home Page"

    use client = getDbClient
    client
    |> getDatabase
    >>= getFirstCollection
    >>= getDocuments
    |> valueOrDefault
    |> x.View

  [<Route("/CreateCollection")>]
  member x.CreateCollection() =

    use client = getDbClient
    client
    |> createDatabase
    |> Async.RunSynchronously
    >>= getOrCreateCollectionSync
    >>= getDocuments
    |> valueOrDefault
    |> Logger.traceRecord
    RedirectResult("/")

  [<Route("/DeleteCollection")>]
  member x.DeleteCollection() = 
    async {
        let! result = getDbClient |> deleteDatabase        
        return RedirectResult("/")  //x.Redirect("/")
    } |> Async.StartAsTask
      

  [<HttpPost>]
  [<Route("")>]
  member x.Index(input:string, collectionLink:string) =
    //let document = JsonValue.Parse(input)
    let document = Newtonsoft.Json.Linq.JObject.Parse(input);

    use client = getDbClient
    client
    |> createDocument collectionLink document
    |> Async.RunSynchronously
    |> ignore

    x.Redirect("/")