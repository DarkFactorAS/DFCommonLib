using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace DFCommonLib.HttpApi.Authorization
{
    /// <summary>
    /// Authorization attribute that requires specific OAuth2 scopes in the JWT token
    /// </summary>
    public class RequireScopeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _requiredScopes;

        public RequireScopeAttribute(params string[] requiredScopes)
        {
            _requiredScopes = requiredScopes ?? throw new ArgumentNullException(nameof(requiredScopes));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get the scope claim from the JWT token
            var scopeClaim = context.HttpContext.User.FindFirst("scope");
            if (scopeClaim == null)
            {
                context.Result = new ForbidResult("No scope claim found in token");
                return;
            }

            // Parse the scopes (typically space-separated)
            var userScopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Check if user has all required scopes
            var missingScopes = _requiredScopes.Where(required => !userScopes.Contains(required)).ToArray();
            if (missingScopes.Any())
            {
                context.Result = new ForbidResult($"Missing required scopes: {string.Join(", ", missingScopes)}");
                return;
            }
        }
    }
}