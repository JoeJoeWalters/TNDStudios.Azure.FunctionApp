using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace FunctionApp1
{
    public class TokenResult
    {
        public Boolean Success { get; set; } = false;
        public ClaimsPrincipal Principal { get; set; } = null;
    }

    public abstract class FunctionSecurityBase
    {
        public virtual String Authority { get; set; }
        public virtual String ValidAudiences { get; set; }
        public virtual String ValidIssuers { get; set; }
        public virtual String SigningKeys { get; set; }

        public virtual Boolean Debugging { get; set; } = false;

        public void InitialiseSecurity(HttpRequest request)
        {
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
        }

        public Boolean HasPermission(String permission)
        {
            return true;
        }

        private TokenResult ValidateToken(String token)
        {
            TokenResult result = new TokenResult() { Success = false };

            // Hide personal information if not in debugging mode
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = Debugging;

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>($"{Authority}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();
            List<String> audienceList = ValidAudiences.Split(',').ToList();
            List<String> issuerList = ValidIssuers.Split(',').ToList();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudiences = audienceList,
                ValidIssuers = issuerList,
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                result.Principal = tokenValidator.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
