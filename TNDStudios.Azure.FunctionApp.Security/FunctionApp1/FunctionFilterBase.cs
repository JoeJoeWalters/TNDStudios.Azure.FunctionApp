using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class TokenResult
    {
        public Boolean Success { get; set; } = false;
    }

    public class FunctionFilterBase : FunctionSecurityBase, IFunctionInvocationFilter
    {
        public FunctionFilterBase()
        {

        }

        /// <summary>
        /// Executed before each function call to scan and process each request
        /// </summary>
        /// <param name="executingContext">The context of the function being called with the binding</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            // Check to see if there is a Http Request component that triggered this request
            HttpRequest request = ExtractHttpRequest(executingContext.Arguments);
            if (request != null)
            {
                // If there was a Http Request then get the bearer token from that request
                String bearerToken = ExtractBearerToken(request);
                if (bearerToken != String.Empty)
                {
                    // If there was a bearer token
                    TokenResult result = ValidateToken(bearerToken);
                    if (result.Success)
                    {

                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private TokenResult ValidateToken(String token)
        {
            TokenResult result = new TokenResult() { Success = false };

            return result;
        }

        private String ExtractBearerToken(HttpRequest request)
        {
            // Get the headers from the Http Request
            if (request.Headers.ContainsKey("Authorization"))
            {
                String authHeader = request.Headers["Authorization"].FirstOrDefault();
                string[] authParts = authHeader.Split(null);
                if (authParts.Length == 2 && authParts[0].Equals("Bearer"))
                    return authParts[1];
                else
                    return String.Empty;
            }
            else
                return String.Empty;
        }

        private HttpRequest ExtractHttpRequest(IReadOnlyDictionary<String, Object> argumentList)
        {
            try
            {
                return (DefaultHttpRequest)argumentList.Where(arg => arg.Value.GetType() == typeof(DefaultHttpRequest)).FirstOrDefault().Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
