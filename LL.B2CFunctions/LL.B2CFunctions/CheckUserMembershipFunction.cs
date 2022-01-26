using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using LL.B2CFunctions.DTOs;
using LL.B2CFunctions.Entities;
using System;
using System.Linq;
using LL.B2CFunctions.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace LL.B2CFunctions
{
    public class CheckUserMembershipFunction
    {
        private readonly IMembershipDetailsDataService _membershipDS;
        private readonly ILogger<CheckUserMembershipFunction> _logger;

        public CheckUserMembershipFunction(IMembershipDetailsDataService membershipDS, ILogger<CheckUserMembershipFunction> logger)
        {
            _membershipDS = membershipDS;
            _logger = logger;
        }

        [FunctionName("CheckUserMembership")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Check user membership" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "text/plain", bodyType: typeof(B2CAttributes), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(ContinueResponse), Description = "The user membership exists")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] B2CAttributes req)
        {
            _logger.LogInformation("CheckUserMembership called");

            if (req.IsValid())
            {
                var membership = await _membershipDS.Get(req.MembershipNumber, req.Email);
                
                if (membership != null)
                {
                    return new OkObjectResult(new ContinueResponse());
                }
            }

            // If the code got to here, there was no matching record in the data table. 
            // We have to return Ok because it needs to be handled by an AAD B2C custom policy
            return new OkObjectResult(new ErrorResponse($"Login failed: Please check your details and try again, if problem persists please contact our support team via email at sign_in_support@londonlibrary.co.uk"));
        }
    }
}
