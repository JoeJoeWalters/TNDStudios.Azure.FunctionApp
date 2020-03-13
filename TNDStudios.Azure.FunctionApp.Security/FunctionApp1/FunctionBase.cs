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
    public class FunctionBase : IFunctionInvocationFilter
    {
        public void SecurityContext()
        {

        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            DefaultHttpRequest request = ExtractHttpRequest(executingContext.Arguments);
            if (request != null)
            {

            }
            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private DefaultHttpRequest ExtractHttpRequest(IReadOnlyDictionary<String, Object> argumentList)
        {
            return (DefaultHttpRequest)argumentList.Where(arg => arg.Value.GetType() == typeof(DefaultHttpRequest)).FirstOrDefault().Value;
        }

        public FunctionBase()
        {

        }
    }
}
