using Microsoft.Framework.DependencyInjection;
using YoloDev.MultiTenant;
using YoloDev.MultiTenant.SubdomainProvider;
using System;

namespace Microsoft.Framework.DependencyInjection
{
    public static class TenantServiceExtensions
    {
        public static ServiceWrapper<TTenant> AddMultiTenant<TTenant>(this IServiceCollection collection)
            where TTenant : class
        {
            collection.AddScoped<ITenantService<TTenant>, TenantService<TTenant>>()
                .AddScoped(services => (ITenantService)services.GetService(typeof(ITenantService<TTenant>)))
                .AddScoped(services => (TenantService)services.GetService(typeof(ITenantService<TTenant>)));

            return new ServiceWrapper<TTenant>(collection);
        }

        public static ServiceWrapper<TTenant> AddSubdomainProvider<TTenant>(this ServiceWrapper<TTenant> collection, Action<SubdomainTenantProviderOptions> configureOptions = null)
            where TTenant : class
        {
            var s = collection.ServiceCollection;
            s.AddOptions();
            s.AddSingleton<ITenantProvider<TTenant> ,SubdomainTenantProvider<TTenant>>();

            if (configureOptions != null)
                s.Configure(configureOptions);

            return collection;
        }
    }
}

namespace YoloDev.MultiTenant
{
    public struct ServiceWrapper<TTenant>
        where TTenant : class
    {
        public ServiceWrapper(IServiceCollection collection)
        {
            ServiceCollection = collection;
        }

        internal IServiceCollection ServiceCollection { get; }
    }
}
