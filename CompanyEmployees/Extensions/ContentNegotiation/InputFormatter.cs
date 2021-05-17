using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace CompanyEmployees.Extensions.ContentNegotiation
{
    public class InputFormatter : TextInputFormatter
    {
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}
