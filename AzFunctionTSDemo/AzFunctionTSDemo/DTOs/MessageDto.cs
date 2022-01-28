using AzFunctionTSDemo.Entities;
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
        public MessageDto(Message message)
        {
            CompanyId = message?.CompanyId ?? "";
            Content = message?.Content ?? "";
            Timestamp = message?.Timestamp ?? DateTimeOffset.MinValue;
            Processed = message?.Processed ?? false;
        }

        public string CompanyId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool Processed { get; set; }
    }
}
