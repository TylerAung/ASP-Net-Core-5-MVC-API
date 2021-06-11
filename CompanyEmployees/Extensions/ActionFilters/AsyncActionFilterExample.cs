using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Extensions.ActionFilters
{
    public class AsyncActionFilterExample : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context
            , ActionExecutionDelegate next)
        {
            // execute any code before the action executes
            var result = await next(); 
            // execute any code after the action executes
            //throw new NotImplementedException();
        }
    }
}
