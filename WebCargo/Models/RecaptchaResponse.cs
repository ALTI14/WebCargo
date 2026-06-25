using System.Text.Json.Serialization;

namespace WebCargo.Models
{
    public class RecaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}