using LL.B2CFunctions.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL.B2CFunctions.Abstractions
{
    public interface IRolesMembershipsDataService : ITableStorageService<RolesMemberships>
    {
        Task<IEnumerable<RolesMemberships>> GetRoles(string? membershipId);
    }
}
