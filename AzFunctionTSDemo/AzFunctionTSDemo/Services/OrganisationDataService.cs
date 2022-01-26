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
    public class OrganisationDataService : TableStorageDataService<Organisation>, IOrganisationDataService
    {
        public OrganisationDataService(IConfiguration configuration, ILogger<OrganisationDataService> logger) : base(configuration, logger)
        {
            TableName = "Roles";
        }

        public Task<Organisation?> Get(string? roleId)
        {
            var roleIdFilter = TableQuery.GenerateFilterCondition(nameof(Organisation.RowKey), QueryComparisons.Equal, roleId?.ToLower());
            var activeFilter = TableQuery.GenerateFilterConditionForBool(nameof(Organisation.Active), QueryComparisons.Equal, true);
            var combinedFilters = TableQuery.CombineFilters(roleIdFilter, TableOperators.And, activeFilter);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}