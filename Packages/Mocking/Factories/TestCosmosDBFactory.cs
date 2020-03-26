using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TNDStudios.Azure.FunctionApp.Mocking.Policies;

namespace TNDStudios.Azure.FunctionApp.Mocking.Factories
{
    public static class TestCosmosDBFactory
    {
        /// <summary>
        /// Convert a list of document from a list of objects, if the 
        /// objects given have an id property then it will be used for the
        /// document id
        /// </summary>
        /// <param name="objects">The objects to initialise the documents</param>
        /// <returns>An initialised list of objects</returns>
        public static List<Document> CreateDocumentList(Stream stream)
        {
            List<Document> documents = new List<Document>(); // The response object
            String json = String.Empty; // The json payload which is empty by default

            // Load the stream in to the json payload variable
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                json = reader.ReadToEnd();

            // Check to see if the object is an array, if it is then enumerate it
            JToken root = JToken.Parse(json);
            if (root != null && 
                root.Type == JTokenType.Array &&
                root.HasValues)
            { 
                foreach (JToken item in JArray.Parse(json))
                {
                    Document document = new Document();
                    document.LoadFrom(new JTokenReader(item));
                    documents.Add(document);
                }
            }
            
            return documents;
        }
        public static List<Document> CreateDocumentList(List<object> objects)
            => objects.Select(obj => obj.ToDocument()).ToList();

        /// <summary>
        /// Convert an object in to a document
        /// </summary>
        /// <returns>A document converted from the object</returns>
        public static Document ToDocument<T>(this T obj) where T : class
        {
            var dynamicDoc = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(obj));
            using (JsonReader reader = new JTokenReader(dynamicDoc))
            {
                var document = new Document();
                document.LoadFrom(reader);
                return document;
            }
        }

        /// <summary>
        /// Get test Document Client for Cosmos DB
        /// </summary>
        /// <returns></returns>
        public static IDocumentClient CreateDocumentClient(
            List<Document> initialisingDocuments, 
            TestExceptionPolicy policy)
        {
            IDocumentClient client = Substitute.For<IDocumentClient>();

            client
                .CreateDocumentAsync(
                    Arg.Any<string>(), 
                    Arg.Any<object>()
                    )
                .Returns(callInfo => 
                    {
                        // If an exception should be raised when trying to write
                        // a document to Cosmos
                        if (policy.WriteException != null)
                            throw policy.WriteException; 

                        Document resource = new Document();
                        //resource.LoadFrom(); callInfo.ArgAt<object>[0]
                        initialisingDocuments.Add(resource);
                        return new ResourceResponse<Document>(resource);
                    });

            client
                .ReadDocumentAsync(
                    Arg.Any<string>(), 
                    Arg.Any<RequestOptions>(), 
                    Arg.Any<CancellationToken>()
                    )
                .Returns(callInfo => 
                    {
                        // If an exception should be raised when trying to read
                        // a document from Cosmos
                        if (policy.ReadException != null)
                            throw policy.ReadException;

                        return new ResourceResponse<Document>();
                    });

            return client;
        }
    }
}
