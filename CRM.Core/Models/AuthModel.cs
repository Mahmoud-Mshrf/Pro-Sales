using System.Text.Json.Serialization;

namespace CRM.Core.Models
{
    public class AuthModel
    {
        public string AccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        [JsonIgnore]
        public bool IsAuthenticated { get; set; }
        //[JsonIgnore]
        //public string Message { get; set; }= string.Empty;
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string,List<string>> Errors { get; set;}
        public AuthModel()
        {
            Errors = null;
        }

    }
}
