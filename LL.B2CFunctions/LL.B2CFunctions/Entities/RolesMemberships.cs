using Microsoft.Azure.Cosmos.Table;

namespace LL.B2CFunctions.Entities
{
    public class RolesMemberships : TableEntity
    {
        public string? RoleId { get; set; }
        public string? MembershipNumber { get; set; }
        public bool Active { get; set; }
    }
}