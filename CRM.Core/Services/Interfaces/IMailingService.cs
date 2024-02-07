using Microsoft.AspNetCore.Http;

namespace CRM.Core.Services.Interfaces
{
    public interface IMailingService
    {
        Task<bool> SendEmailAsync(string mailTo, string subject, string content, IList<IFormFile> attchments = null);
    }
}
