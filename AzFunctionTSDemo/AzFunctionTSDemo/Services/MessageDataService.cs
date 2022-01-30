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

        public Task<IEnumerable<Message>> Get(string? companyId,
                                              DateTimeOffset? fromTime,
                                              DateTimeOffset? toTime,
                                              bool? processed)
        {
            List<string> filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                filters.Add(TableQuery.GenerateFilterCondition(nameof(Message.CompanyId), QueryComparisons.Equal, companyId));
            }
            if (fromTime.HasValue)
            {
                filters.Add(TableQuery.GenerateFilterConditionForDate(nameof(Message.Timestamp), QueryComparisons.GreaterThanOrEqual, fromTime!.Value));
            }
            if (toTime.HasValue)
            {
                filters.Add(TableQuery.GenerateFilterConditionForDate(nameof(Message.Timestamp), QueryComparisons.LessThanOrEqual, toTime!.Value));
            }
            if (processed.HasValue)
            {
                filters.Add(TableQuery.GenerateFilterConditionForBool(nameof(Message.Processed), QueryComparisons.Equal, processed!.Value));
            }

            var combinedFilters = string.Join(" and ", filters);

            return RetrieveCollectionAsync(combinedFilters);
        }
    }
}