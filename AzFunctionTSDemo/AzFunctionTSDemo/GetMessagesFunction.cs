using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using AzFunctionTSDemo.DTOs;
using AzFunctionTSDemo.Abstractions;

namespace AzFunctionTSDemo
{
    public class GetMessagesFunction
    {
        private readonly ICompanyDataService _companyDS;
        private readonly IMessageDataService _messageDS;
        private readonly ILogger<GetMessagesFunction> _logger;

        public GetMessagesFunction(ICompanyDataService companyDS,
                                   IMessageDataService messageDS,
                                   ILogger<GetMessagesFunction> logger)
        {
            _companyDS = companyDS;
            _messageDS = messageDS;
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

            var messages = await _messageDS.Get(req.CompanyId, req.FromTime, req.ToTime, req.Processed);
            if (!messages.Any())
            {
                return new OkObjectResult(null);
            }

            List<MessageDto> messagesFound = new();
            foreach (var message in messages)
            {
                var company = await _companyDS.Get(message.CompanyId);
                if (company?.Active == true)
                {
                    messagesFound.Add(new MessageDto(message));
                }    
            }

            return new OkObjectResult(messagesFound);
        }
    }
}
