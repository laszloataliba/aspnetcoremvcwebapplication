﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Security.Claims;

namespace DevIO.App.Extensions
{
    public class CustomAuthorization
    {
        public static bool ValidateUserClaims(HttpContext context, string claimName, string claimValue)
        {
            return
                (
                    context.User.Identity.IsAuthenticated
                        &&
                    context.User.Claims.Any(claim =>
                        claim.Type == claimName
                            &&
                        claim.Value.Contains(claimValue)
                    )
                );
        }
    }

    public class ClaimsAuthorizerAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizerAttribute(string claimName, string claimValue) :
            base(typeof(RequirementClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }

    public class RequirementClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequirementClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            area = "Identity",
                            page = "/Account/Login",
                            ReturnUrl = context.HttpContext.Request.Path.ToString()
                        }
                    )
                );
                return;
            }

            if (!CustomAuthorization.ValidateUserClaims(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
