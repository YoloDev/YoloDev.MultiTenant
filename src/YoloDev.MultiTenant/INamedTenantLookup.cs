using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoloDev.MultiTenant
{
    public interface INamedTenantLookup<TTenant>
    {
        Task<TTenant> Lookup(string name);
    }
}
