using AzFunctionTSDemo.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.Abstractions
{
    public interface IMessageDataService : ITableStorageService<Message>
    {
        Task<IEnumerable<Message>> Get(string fromOrganisationId,
                                       string? toOrganisationId,
                                       DateTimeOffset? fromTime,
                                       DateTimeOffset? toTime,
                                       bool? processed);
    }
}
