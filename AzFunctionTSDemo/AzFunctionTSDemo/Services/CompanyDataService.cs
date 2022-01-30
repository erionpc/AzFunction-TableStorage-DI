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
    public class CompanyDataService : TableStorageDataService<Company>, ICompanyDataService
    {
        public CompanyDataService(IConfiguration configuration, ILogger<CompanyDataService> logger) : base(configuration, logger)
        {
            TableName = "Company";
        }

        public Task<Company?> Get(string? companyId)
        {
            var companyIdFilter = TableQuery.GenerateFilterCondition(nameof(Company.RowKey), QueryComparisons.Equal, companyId);
            var activeFilter = TableQuery.GenerateFilterConditionForBool(nameof(Company.Active), QueryComparisons.Equal, true);
            var combinedFilters = TableQuery.CombineFilters(companyIdFilter, TableOperators.And, activeFilter);

            return RetrieveEntityAsync(combinedFilters);
        }
    }
}