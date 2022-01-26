using LL.B2CFunctions.Abstractions;
using LL.B2CFunctions.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LL.B2CFunctions.Services
{
    public class MembershipDetailsDataService : TableStorageDataService<MembershipDetails>, IMembershipDetailsDataService
    {
        public MembershipDetailsDataService(IConfiguration configuration, ILogger<MembershipDetailsDataService> logger) : base(configuration, logger)
        {
            TableName = "MembershipDetails";
        }

        public Task<MembershipDetails?> Get(string? membershipId, string? emailAddress)
        {
            var membershipNumberFilter = TableQuery.GenerateFilterCondition(nameof(MembershipDetails.membership_number), QueryComparisons.Equal, membershipId?.ToLower());
            var emailFilter = TableQuery.GenerateFilterCondition(nameof(MembershipDetails.email), QueryComparisons.Equal, emailAddress?.ToLower());
            var combinedFilters = TableQuery.CombineFilters(emailFilter, TableOperators.And, membershipNumberFilter);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}