using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace TNDStudios.Azure.FunctionApp.Security
{
    public abstract class FunctionSecurityBase<T>
    {
        public abstract String Authority { get; set; }
        public abstract String ValidAudiences { get; set; }
        public abstract String ValidIssuers { get; set; }
        public abstract String SigningKeys { get; set; }

        public virtual Boolean Debugging { get; set; } = false;

        public virtual SecurityResult<T> SecurityContext { get; set; } = new SecurityResult<T>();

        /// <summary>
        /// Starup and initialisation of the security context usually called at the start of the Azure function
        /// </summary>
        /// <param name="request">The request sent to the Azure Function</param>
        public SecurityResult<T> InitialiseSecurity(HttpRequest request)
        {
            SecurityResult<T> result = new SecurityResult<T>();
            if (request != null)
            {
                // If there was a Http Request then get the bearer token from that request
                String bearerToken = ExtractBearerToken(request);
                if (bearerToken != String.Empty)
                {
                    // If there was a bearer token
                    TokenResult tokenResult = ValidateToken(bearerToken);
                    if (tokenResult.Success)
                    {
                        // Translate the permissions list from the resulting claims principal
                        result.Permissions = new List<T>() { };
                        result.Initialised = true;

                        // Set the local context in this class as it can also act as a base context along with returning the result
                        SecurityContext = result;
                    }
                }

                return result;
            }
            else
                throw new Exception("Cannot initialise security context as there is no Http Context to resolve it from");
        }

        /// <summary>
        /// Does the current context have permissions of a given type
        /// </summary>
        /// <param name="value">The enum item of the pre-defined type</param>
        /// <returns>If the context has the permission</returns>
        public Boolean HasPermission(T value) => SecurityContext.HasPermission(value);

        /// <summary>
        /// Validate the token taken from the http context
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /*
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
        */
    }
}
