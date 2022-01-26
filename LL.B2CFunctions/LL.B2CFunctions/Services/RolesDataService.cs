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
    public class RolesDataService : TableStorageDataService<Roles>, IRolesDataService
    {
        public RolesDataService(IConfiguration configuration, ILogger<RolesDataService> logger) : base(configuration, logger)
        {
            TableName = "Roles";
        }

        public Task<Roles?> Get(string? roleId)
        {
            var roleIdFilter = TableQuery.GenerateFilterCondition(nameof(Roles.RowKey), QueryComparisons.Equal, roleId?.ToLower());
            var activeFilter = TableQuery.GenerateFilterConditionForBool(nameof(Roles.Active), QueryComparisons.Equal, true);
            var combinedFilters = TableQuery.CombineFilters(roleIdFilter, TableOperators.And, activeFilter);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}