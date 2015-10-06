using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoloDev.MultiTenant.SubdomainProvider
{
    public class SubdomainTenantProviderOptions
    {
        public string BaseDomain { get; set; } = null;

        public bool AllowMultipleLevels { get; set; } = false;

        public bool RequireValidTenant { get; set; } = true;
    }
}
