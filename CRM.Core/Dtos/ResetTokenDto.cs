using System.Text.Json.Serialization;

namespace CRM.Core.Dtos
{
    public class ResetTokenDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public ResetTokenDto()
        {
            Errors = null;
        }

    }
}
