using Microsoft.Azure.Cosmos.Table;

namespace AzFunctionTSDemo.Entities
{
    public class Message : TableEntity
    {
        public string? CompanyId { get; set; }
        public string? Content { get; set; }
        public bool Processed { get; set; }
    }
}
