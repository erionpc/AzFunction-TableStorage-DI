using Microsoft.Azure.Cosmos.Table;

namespace AzFunctionTSDemo.Entities
{
    public class Organisation : TableEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}
