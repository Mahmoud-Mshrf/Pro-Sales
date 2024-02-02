namespace CRM.Services.Interfaces
{
    public interface IMailingService
    {
        Task<bool> SendEmailAsync(string mailTo, string subject, string content, IList<IFormFile> attchments = null);
    }
}
