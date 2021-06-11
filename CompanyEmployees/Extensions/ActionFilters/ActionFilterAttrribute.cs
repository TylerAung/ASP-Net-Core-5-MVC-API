using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Extensions.ActionFilters
{
    public class ActionFilterExample : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        { // our code before action executes 
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes 
        }
    }
}
