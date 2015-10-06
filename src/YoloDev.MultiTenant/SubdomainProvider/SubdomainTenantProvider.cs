using Microsoft.Framework.OptionsModel;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Primitives;
using System.Text.RegularExpressions;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace YoloDev.MultiTenant.SubdomainProvider
{
    public class SubdomainTenantProvider<TTenant> : ITenantProvider<TTenant>
        where TTenant : class
    {
        readonly static Tuple<TTenant, bool> Failure = Tuple.Create(default(TTenant), false);
        readonly static Regex PortNumberRegex = new Regex(":\\d{1,5}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        readonly SubdomainTenantProviderOptions _options;
        readonly ILogger _logger;

        public SubdomainTenantProvider(IOptions<SubdomainTenantProviderOptions> options, ILoggerFactory factory)
        {
            _options = options.Value;
            _logger = factory.CreateLogger<SubdomainTenantProvider<TTenant>>();
        }

        public async Task<Tuple<TTenant, bool>> TryGetTenant(HttpContext context)
        {
            StringValues hostNameHeader;
            if (!context.Request.Headers.TryGetValue("Host", out hostNameHeader))
                return Failure;

            if (hostNameHeader.Count != 1)
                return Failure;

            var hostName = hostNameHeader[0];
            if (string.IsNullOrEmpty(hostName))
                return Failure;

            // Remove port number
            hostName = PortNumberRegex.Replace(hostName, "");

            string subDomain;
            if (!string.IsNullOrEmpty(_options.BaseDomain))
            {
                if (!hostName.EndsWith("." + _options.BaseDomain))
                    return Failure;

                subDomain = hostName.Substring(0, hostName.Length - _options.BaseDomain.Length - 1);
            }
            else
            {
                // Assuming the normal subdomain.dommain.topdomain layout.
                // Also, default exception for "localhost". Anything else
                // will fail and need to be configured using BaseDomain
                // on the options object.
                var parts = hostName.Split('.');

                if (parts[parts.Length - 1] == "localhost")
                {
                    if (parts.Length <= 1)
                        return Failure;

                    subDomain = string.Join(".", new ArraySegment<string>(parts, 0, parts.Length - 1));
                }
                else
                {
                    if (parts.Length <= 2)
                        return Failure;

                    subDomain = string.Join(".", new ArraySegment<string>(parts, 0, parts.Length - 2));
                }
            }

            if (subDomain.Contains(".") && !_options.AllowMultipleLevels)
                return Failure;

            _logger.LogDebug("Subdomain is {0}. Looking up tenant.", subDomain);
            var lookup = context.RequestServices.GetRequiredService<INamedTenantLookup<TTenant>>();

            var tenant = await lookup.Lookup(subDomain);
            if (tenant == null)
            {
                if (_options.RequireValidTenant)
                {
                    _logger.LogCritical("Valid tenant for subdomain {0} not found.", subDomain);
                    context.Response.StatusCode = 404;
                    return null;
                }
                else
                {
                    return Failure;
                }
            }

            return Tuple.Create(tenant, true);
        }
    }
}
