using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//needed to manually add the using for this: using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using DatingApp.API.Models;

namespace DatingApp.API
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public LogUserActivity()
        {

        }

        /// <summary>
        /// Whenever the user does something it will update and save new User.LastActive
        /// </summary>
        /// <param name="context">If we want to do something whilst the action is being executed</param>
        /// <param name="next">If we want to do something AFTER the action has finished</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext resultContext = await next();

            int userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //Fetching the repository service out of the current HttpContext
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            User user = await repo.GetUser(userId);
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAll();
        }
    }
}
