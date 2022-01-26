using Microsoft.Azure.Cosmos.Table;

namespace LL.B2CFunctions.Entities
{
    public class MembershipDetails : TableEntity
    {
        public string? email { get; set; }

        public string? membership_number { get; set; }
    }
}
