using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : Attribute, IActionFilter
    {
        private readonly string[] _allowedRoles;

        // Allows specifying one or more roles
        
        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId");
            var userRole = session.GetString("UserRole");

            // Checks if the user is logged in
            if (!userId.HasValue || string.IsNullOrEmpty(userRole))
            {
                context.Result = new ObjectResult(new { message = "Authentication required. Please log in." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Checks if the logged-in user has the required role
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(new { message = $"Access denied. Required role: {string.Join(" or ", _allowedRoles)}" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}