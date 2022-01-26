using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzFunctionTSDemo.DTOs
{
    public sealed class MessageDto
    {
        public string? FromOrganisationId { get; set; }
        public string? ToOrganisationId { get; set; }
        public string? Content { get; set; }
        public bool Processed { get; set; }
    }
}
