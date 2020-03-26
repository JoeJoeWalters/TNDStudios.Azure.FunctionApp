using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using TNDStudios.Azure.FunctionApp.Security;

namespace FunctionApp1
{
    public enum Permissions
    {
        [Description("00001-00001-00001")]
        TestPermission,
    }

    public class TestFunctions : FunctionSecurityBase<Permissions>
    {
        /// <summary>
        /// Enforced requirements to defined audience, keys etc. for security
        /// </summary>
        public override string Authority { get; set; } = String.Empty;
        public override string ValidAudiences { get; set; } = String.Empty;
        public override string ValidIssuers { get; set; } = String.Empty;
        public override string SigningKeys { get; set; } = String.Empty;

        public TestFunctions() : base()
        {

        }

        [FunctionName("Function1")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            InitialiseSecurity(req);

            return HasPermission(Permissions.TestPermission)
                ? (ActionResult)new OkObjectResult($"Hello, Tester")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
