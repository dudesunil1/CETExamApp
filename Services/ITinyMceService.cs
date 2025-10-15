namespace CETExamApp.Services
{
    public interface ITinyMceService
    {
        Task<string?> GetActiveApiKeyAsync();
    }
}

