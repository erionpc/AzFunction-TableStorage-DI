using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.DTOs
{
    public sealed class GetMessagesRequestDto
    {
        public string? CompanyId { get; set; }
        public DateTimeOffset? FromTime { get; set; }
        public DateTimeOffset? ToTime { get; set; }
        public bool? Processed { get; set; }
    }
}