using CETExamApp.Models;
using Microsoft.Extensions.Options;

namespace CETExamApp.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;

        public TenantService(IOptions<TenantSettings> tenantSettings)
        {
            _tenantSettings = tenantSettings.Value;
        }

        public TenantSettings GetTenantSettings()
        {
            return _tenantSettings;
        }
    }
}

