using YoloDev.MultiTenant;

namespace Microsoft.AspNet.Builder
{
    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseMultiTenant(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TenantMiddleware>();
        }

        public static IApplicationBuilder UseRequireTenant(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequireTenantMiddleware>();
        }
    }
}
