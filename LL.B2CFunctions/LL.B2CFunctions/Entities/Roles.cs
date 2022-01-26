using Microsoft.Azure.Cosmos.Table;

namespace LL.B2CFunctions.Entities
{
    public class Roles : TableEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}
