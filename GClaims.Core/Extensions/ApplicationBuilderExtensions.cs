using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace GClaims.Core.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a middleware delegate defined in-line to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="middleware">A function that handles the request or calls the given next function.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder Use(this IApplicationBuilder app, Func<HttpContext, Func<Task>, Task> middleware, bool useThis)
    {
        return app.Use(next =>
        {
            return context =>
            {
                Func<Task> simpleNext = () => next(context);
                return middleware(context, simpleNext);
            };
        });
    }
}