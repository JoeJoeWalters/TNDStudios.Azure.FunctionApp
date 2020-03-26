using System;
using System.Security.Claims;

namespace TNDStudios.Azure.FunctionApp.Security
{
    public class TokenResult
    {
        public Boolean Success { get; set; } = false;
        public ClaimsPrincipal Principal { get; set; } = null;
    }
}
