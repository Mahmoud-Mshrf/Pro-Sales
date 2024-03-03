using System.Text.Json.Serialization;

namespace CRM.Core.Dtos
{
    public class ResultDto
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public ResultDto()
        {
            Errors = null;
        }

    }
}
