using System.Text.Json.Serialization;

namespace CRM.Core.Dtos
{
    public class ResultDto
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
