namespace CRM.Core.Dtos
{
    public class ResetTokenDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}
