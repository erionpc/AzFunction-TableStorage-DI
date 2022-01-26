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
    public class RolesMembershipsDataService : TableStorageDataService<RolesMemberships>, IRolesMembershipsDataService
    {
        public RolesMembershipsDataService(IConfiguration configuration, ILogger<RolesMembershipsDataService> logger) : base(configuration, logger)
        {
            TableName = "RolesMemberships";
        }

        public Task<IEnumerable<RolesMemberships>> GetRoles(string? membershipId)
        {
            var membershipNumberFilter = TableQuery.GenerateFilterCondition(nameof(RolesMemberships.MembershipNumber), QueryComparisons.Equal, membershipId?.ToLower());
            var activeFilter = TableQuery.GenerateFilterConditionForBool(nameof(RolesMemberships.Active), QueryComparisons.Equal, true);
            var combinedFilters = TableQuery.CombineFilters(membershipNumberFilter, TableOperators.And, activeFilter);

            return RetrieveCollectionAsync(combinedFilters);
        }
    }
}