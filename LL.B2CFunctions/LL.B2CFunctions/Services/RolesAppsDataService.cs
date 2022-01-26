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
    public class RolesAppsDataService : TableStorageDataService<RolesApps>, IRolesAppsDataService
    {
        public RolesAppsDataService(IConfiguration configuration, ILogger<RolesAppsDataService> logger) : base(configuration, logger)
        {
            TableName = "RolesApps";
        }

        public Task<RolesApps?> Get(IEnumerable<string?> roleIds, string? tenant, string? appId)
        {            
            if (!roleIds.Any())
            {
                return Task.FromResult(null as RolesApps);
            }

            var combinedRoleFilters = TableQuery.GenerateFilterCondition(nameof(RolesApps.RoleId), QueryComparisons.Equal, roleIds!.ElementAt(0)?.ToLower());

            for (int i = 1; i < roleIds.Count(); i++)
            {
                var roleFilter = TableQuery.GenerateFilterCondition(nameof(RolesApps.RoleId), QueryComparisons.Equal, roleIds!.ElementAt(i)?.ToLower());
                combinedRoleFilters = TableQuery.CombineFilters(combinedRoleFilters, TableOperators.Or, roleFilter);
            }

            var tenantFilter = TableQuery.GenerateFilterCondition(nameof(RolesApps.Tenant), QueryComparisons.Equal, tenant?.ToLower());
            var appIdFilter = TableQuery.GenerateFilterCondition(nameof(RolesApps.AppId), QueryComparisons.Equal, appId?.ToLower());
            var activeFilter = TableQuery.GenerateFilterConditionForBool(nameof(RolesApps.Active), QueryComparisons.Equal, true);

            var combinedFilters1 = TableQuery.CombineFilters(combinedRoleFilters, TableOperators.And, tenantFilter);
            var combinedFilters2 = TableQuery.CombineFilters(appIdFilter, TableOperators.And, activeFilter);
            var combinedFilters = TableQuery.CombineFilters(combinedFilters1, TableOperators.And, combinedFilters2);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}