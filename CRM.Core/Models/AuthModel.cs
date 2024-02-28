using System.Text.Json.Serialization;

namespace CRM.Core.Models
{
    public class AuthModel
    {
        public string AccessToken { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }= string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
