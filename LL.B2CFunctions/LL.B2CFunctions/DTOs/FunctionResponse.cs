using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL.B2CFunctions.DTOs
{
    public class FunctionResponse
    {
        public string Version { get; }
        public string Action { get; protected set; } = "";
        public string UserMessage { get; protected set; } = "";

        public FunctionResponse()
        {
            Version = "2.0.0";
        }
    }
}
