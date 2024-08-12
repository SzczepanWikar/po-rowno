using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware.UserFetching
{
    public sealed class UserFetchingMiddleware : IMiddleware
    {
        private readonly UserFetcher _fetcher;

        public UserFetchingMiddleware(UserFetcher fetcher)
        {
            _fetcher = fetcher;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context
                    .User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    ?.Value;

                if (userId != null)
                {
                    var user = await _fetcher.FetchAsync(Guid.Parse(userId));
                    context.Items["User"] = user;
                }
            }

            await next.Invoke(context);
        }
    }
}
