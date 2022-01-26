using AzFunctionTSDemo.Abstractions;
using AzFunctionTSDemo.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.Services
{
    public class MessageDataService : TableStorageDataService<Message>, IMessageDataService
    {
        public MessageDataService(IConfiguration configuration, ILogger<MessageDataService> logger) : base(configuration, logger)
        {
            TableName = "Message";
        }

        public Task<Message?> Get(string? membershipId, string? emailAddress)
        {
            var membershipNumberFilter = TableQuery.GenerateFilterCondition(nameof(Message.MembershipNumber), QueryComparisons.Equal, membershipId?.ToLower());
            var emailFilter = TableQuery.GenerateFilterCondition(nameof(Message.Email), QueryComparisons.Equal, emailAddress?.ToLower());
            var combinedFilters = TableQuery.CombineFilters(emailFilter, TableOperators.And, membershipNumberFilter);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}