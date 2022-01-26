using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LL.B2CFunctions.DTOs
{
    public sealed class B2CAttributes
    {
        public string? Email { get; set; }

        public string? MembershipNumber { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(MembershipNumber);
    }
}