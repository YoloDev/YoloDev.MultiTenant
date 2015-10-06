using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;

namespace YoloDev.MultiTenant
{
    class TenantMiddleware
    {
        readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TenantService service)
        {
            if (!await service.SetTenant(context))
                return;

            await _next(context);
        }
    }
}
