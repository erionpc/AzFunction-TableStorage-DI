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
using AzFunctionTSDemo.DTOs;
using AzFunctionTSDemo.Abstractions;
using System.Linq;
using System.Collections.Generic;

namespace AzFunctionTSDemo
{
    public class GetMessagesFunction
    {
        private readonly IOrganisationDataService _organisationDS;
        private readonly IMessageDataService _messageDS;
        private readonly ILogger<GetMessagesFunction> _logger;

        public GetMessagesFunction(IOrganisationDataService rolesDS,
                                   IMessageDataService rolesAppsDS,
                                   ILogger<GetMessagesFunction> logger)
        {
            _organisationDS = rolesDS;
            _messageDS = rolesAppsDS;
            _logger = logger;
        }

        [FunctionName("GetMessages")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Get messages" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "text/plain", bodyType: typeof(GetMessagesRequestDto), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<MessageDto>), Description = "Messages")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] GetMessagesRequestDto req)
        {
            _logger.LogInformation("GetMessages called");
                        
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
                var role = await _organisationDS.Get(roleMembership.RoleId);
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
            var appRole = await _messageDS.Get(activeRoles, req.Tenant, req.AppId);
            
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
