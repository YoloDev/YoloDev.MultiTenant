using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoloDev.MultiTenant
{
    public interface ITenantProvider<TTenant>
    {
        Task<Tuple<TTenant, bool>> TryGetTenant(HttpContext context);
    }
}
