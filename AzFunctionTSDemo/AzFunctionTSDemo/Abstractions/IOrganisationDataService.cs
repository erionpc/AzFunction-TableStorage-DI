using AzFunctionTSDemo.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.Abstractions
{
    public interface IOrganisationDataService : ITableStorageService<Organisation>
    {
        Task<Organisation?> Get(string? organisationId);
    }
}
