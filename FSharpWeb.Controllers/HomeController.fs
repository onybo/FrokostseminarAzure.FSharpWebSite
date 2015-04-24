namespace FSharpWeb.Controllers

open System
open System.Web
open System.Web.Routing
open System.Web.Mvc
open DocumentDatabase
open DocumentCollection
open Document
open DocumentDbSample.Core
open Microsoft.Azure.Documents.Client
open Microsoft.Azure.Documents
open Chessie.ErrorHandling

type HomeController() =
  inherit Controller()

  let getDbClient = 
    let uri = Uri "https://olavsstorage.documents.azure.com:443/"
    let authKey = toSecureString (System.Configuration.ConfigurationManager.AppSettings.Get("AuthKey"))
    new DocumentClient (uri, authKey=authKey)

  // Pipelining of Results. Bind Operator >>=
  [<Route("/CreateCollection")>]
  member x.CreateCollection() =

    use client = getDbClient
    ok client
    >>= tryCreateDatabase
    >>= getOrCreateCollectionSync client
    >>= getDocuments client
    |> Logger.traceResult 
    RedirectResult("/")

  member private x.SuccessView collectionAndDocuments =
    let (collection:DocumentCollection,documents:Collections.Generic.List<Person>),_ = collectionAndDocuments
    x.View({id=collection.Id; selfLink=collection.SelfLink; documentsLink=collection.DocumentsLink; documents=documents})

  member private x.FailView result =
   x.View({id="no database"; selfLink="no collection"; documentsLink="no documents"; documents=Seq.empty})

  // Trial computation expression. Collect the partial results of a "pipeline"
  [<HttpGet>]
  [<Route("")>]
  member x.Index() =
    use client = getDbClient

    trial {
      let! collection = client
                        |> getDatabase
                        >>= getFirstCollection client
      let! documents = collection
                       |> getDocuments client
      return (collection, documents)
    } |> either x.SuccessView x.FailView


  //async methods in F#
  [<Route("/DeleteCollection")>]
  member x.DeleteCollection() = 
    async {
        let! result = getDbClient |> deleteDatabase        
        return RedirectResult("/") 
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