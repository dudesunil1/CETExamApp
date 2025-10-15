using CETExamApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CETExamApp.Services
{
    public class TinyMceService : ITinyMceService
    {
        private readonly ApplicationDbContext _context;

        public TinyMceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetActiveApiKeyAsync()
        {
            var activeKey = await _context.ApiKeys
                .Where(k => !k.InActive)
                .OrderByDescending(k => k.CreatedDate)
                .FirstOrDefaultAsync();

            return activeKey?.ApiKeyValue;
        }
    }
}

