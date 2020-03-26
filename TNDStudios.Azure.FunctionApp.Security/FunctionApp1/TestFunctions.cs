using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class TestFunctions : FunctionSecurityBase<Permissions>
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

            return HasPermission(Permissions.TestPermission)
                ? (ActionResult)new OkObjectResult($"Hello, Tester")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
