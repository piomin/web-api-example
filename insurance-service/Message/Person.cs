using System.Text.Json.Serialization;

namespace insurance_service.Message
{
    public class Person
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }
    }
}