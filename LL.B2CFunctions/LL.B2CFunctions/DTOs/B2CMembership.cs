using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LL.B2CFunctions.DTOs
{
    public sealed class B2CMembership
    {
        public string? MembershipNumber { get; set; }
        public string? Tenant { get; set; }
        public string? AppId { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(MembershipNumber) && !string.IsNullOrWhiteSpace(Tenant) && !string.IsNullOrWhiteSpace(AppId);
    }
}
