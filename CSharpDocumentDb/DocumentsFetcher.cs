using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CSharpDocumentDb
{
    public class DocumentsFetcher
    {
        private static string EndpointUrl = "<your endpoint URI>";
        private static string AuthorizationKey = "<your key>";

        public IEnumerable<Document> GetDocuments()
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var database = client.CreateDatabaseAsync(
                new Database
                {
                    Id = "OlavsDemoDB"
                }).Result.Resource;
            var documentCollection = client.CreateDocumentCollectionAsync(database.CollectionsLink,
                new DocumentCollection
                {
                    Id = "Persons"
                }).Result.Resource;
            return client.CreateDocumentQuery<Document>(documentCollection.DocumentsLink).ToList();
        }
    }
}
