using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.WebJobs.Host;
using System.Threading;

namespace FunctionApp1
{
    public class TestFunctions : FunctionSecurityBase
    {
        public TestFunctions() : base()
        {

        }

        [FunctionName("Function1")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            base.InitialiseSecurity(req);

            return HasPermission("testpermission")
                ? (ActionResult)new OkObjectResult($"Hello, Tester")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
