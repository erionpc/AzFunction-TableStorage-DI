using Microsoft.Azure.Cosmos.Table;

namespace LL.B2CFunctions.Entities
{
    public class RolesApps : TableEntity
    {
        public string? RoleId { get; set; }
        public string? AppId { get; set; }
        public string? Tenant { get; set; }
        public bool Active { get; set; }
    }
}
