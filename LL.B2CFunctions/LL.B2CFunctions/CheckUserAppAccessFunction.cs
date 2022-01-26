using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Net;
using LL.B2CFunctions.DTOs;
using LL.B2CFunctions.Abstractions;
using System.Linq;
using System.Collections.Generic;

namespace LL.B2CFunctions
{
    public class CheckUserAppAccessFunction
    {
        private readonly IRolesMembershipsDataService _rolesMembershipsDS;
        private readonly IRolesDataService _rolesDS;
        private readonly IRolesAppsDataService _rolesAppsDS;
        private readonly ILogger<CheckUserAppAccessFunction> _logger;

        public CheckUserAppAccessFunction(IRolesMembershipsDataService rolesMembershipsDS,
                                          IRolesDataService rolesDS,
                                          IRolesAppsDataService rolesAppsDS,
                                          ILogger<CheckUserAppAccessFunction> logger)
        {
            _rolesMembershipsDS = rolesMembershipsDS;
            _rolesDS = rolesDS;
            _rolesAppsDS = rolesAppsDS;
            _logger = logger;
        }

        [FunctionName("CheckUserAppAccess")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Check user app access" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "text/plain", bodyType: typeof(B2CMembership), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(ContinueResponse), Description = "The user has access to the app")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] B2CMembership req)
        {
            _logger.LogInformation("CheckUserAppAccess called");

            // Return error if the request is not valid
            if (!req.IsValid())
            {
                return new OkObjectResult(new ErrorResponse("The request is invalid"));
            }
            
            // Return error if the membership number is not linked to any roles
            var roles = await _rolesMembershipsDS.GetRoles(req.MembershipNumber);
            if (!roles.Any())
            {
                return new OkObjectResult(new ErrorResponse("The user is not linked to any roles"));
            }

            // Return error if the membership number is not linked to any active roles
            List<string?> activeRoles = new();
            foreach (var roleMembership in roles)
            {
                var role = await _rolesDS.Get(roleMembership.RoleId);
                if (role != null)
                {
                    activeRoles.Add(roleMembership!.RoleId);
                }
            }
            if (!activeRoles.Any())
            {
                return new OkObjectResult(new ErrorResponse("The user is not linked to any roles"));
            }

            // Search the active role(s) for access to the destination app
            var appRole = await _rolesAppsDS.Get(activeRoles, req.Tenant, req.AppId);
            
            if (appRole != null)
            {
                return new OkObjectResult(new ContinueResponse());
            }
            else
            {
                return new OkObjectResult(new ErrorResponse("The user doesn't have access to the app"));
            }
        }
    }
}
