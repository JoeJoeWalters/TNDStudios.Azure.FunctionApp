using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TNDStudios.Azure.FunctionApp.Mocking.Extensions;

namespace TNDStudios.Azure.FunctionApp.Mocking.Factories
{
    public static class TestHttpFactory
    {
        public static DefaultHttpRequest CreateHttpRequest()
            => CreateHttpRequest(String.Empty, String.Empty, String.Empty, new HeaderDictionary());

        public static DefaultHttpRequest CreateHttpRequest(
            String queryStringKey,
            String queryStringValue)
            => CreateHttpRequest(queryStringKey, queryStringValue, String.Empty, new HeaderDictionary());

        public static DefaultHttpRequest CreateHttpRequest(
            String queryStringKey,
            String queryStringValue,
            String body)
            => CreateHttpRequest(queryStringKey, queryStringValue, body, new HeaderDictionary());

        public static DefaultHttpRequest CreateHttpRequest(
            String queryStringKey,
            String queryStringValue,
            String body,
            IHeaderDictionary headers)
        {
            // Do we need to implement a query collection to hold querystring
            // elements to process?
            QueryCollection query = new QueryCollection();
            if ((queryStringKey ?? String.Empty) != String.Empty &&
                (queryStringValue ?? String.Empty) != String.Empty)
            {
                query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue));
            }

            // Generate the request based on the parameters calculated
            DefaultHttpRequest request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = query,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(body ?? String.Empty))
            };

            // Calculate the headers
            headers = headers ?? new HeaderDictionary();
            foreach (var header in headers)
                request.Headers.Add(header);

            // Send the request back
            return request;
        }

        private static Dictionary<string, StringValues> CreateDictionary(string key, string value)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key, value }
            };
            return qs;
        }

        /// <summary>
        /// Convert the action result of an azure http function to a http result set of data
        /// </summary>
        /// <param name="functionResult">The Task based outout from a http azure function</param>
        /// <returns>The constructed http response</returns>
        public static HttpResponseMessage ToHttpResponseMessage(this Task<IActionResult> taskResult)
            => taskResult.Result.ToHttpResponseMessage();
        public static HttpResponseMessage ToHttpResponseMessage(this IActionResult actionResult)
            => new HttpResponseMessage(actionResult.GetHttpStatusCode())
            {
                Content = new StringContent(actionResult.GetHttpValue())
            };

        /// <summary>
        /// Simplify the content retrieval for testing
        /// </summary>
        /// <typeparam name="TValue">The type required</typeparam>
        /// <param name="content">The HttpContent being read from</param>
        /// <returns>The content cast to the right type</returns>
        public static String Get(this HttpContent content) => content.Get<String>(); // String "get" is the default
        public static T Get<T>(this HttpContent content)
        {
            Object result;
            
            switch (typeof(T).ShortName())
            {
                case "string":
                    result = content.ReadAsStringAsync().Result;
                    break;

                default:
                    return default(T);
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Convert the action result of an azure http function to a http status code
        /// </summary>
        /// <param name="functionResult">The Task based outout from a http azure function</param>
        /// <returns>The http status code</returns>
        public static HttpStatusCode GetHttpStatusCode(this Task<IActionResult> functionResult)
            => functionResult.Result.GetHttpStatusCode();
        public static HttpStatusCode GetHttpStatusCode(this IActionResult functionResult)
        {
            try
            {
                return (HttpStatusCode)functionResult
                    .GetType()
                    .GetProperty("StatusCode")
                    .GetValue(functionResult, null);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Convert the action result of an azure http function to a value
        /// </summary>
        /// <param name="functionResult">The Task based outout from a http azure function</param>
        /// <returns>The http value</returns>
        public static String GetHttpValue(this Task<IActionResult> functionResult)
            => functionResult.Result.GetHttpValue();
        public static String GetHttpValue(this IActionResult functionResult)
        {
            try
            {
                return (String)functionResult
                    .GetType()
                    .GetProperty("Value")
                    .GetValue(functionResult, null);
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
