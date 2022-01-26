using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL.B2CFunctions.DTOs
{
    public class ErrorResponse : FunctionResponse
    {
        public int Status { get; }

        public ErrorResponse(string message) : base()
        {
            Action = "ValidationError";
            UserMessage = message;
            Status = 400;
        }
    }
}
