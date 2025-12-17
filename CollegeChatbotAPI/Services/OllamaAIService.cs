using System.Text;
using System.Text.Json;

namespace CollegeChatbotAPI.Services
{
    public class OllamaAIService : IAIService
    {
        private readonly HttpClient _httpClient;

        public OllamaAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        public async Task<string> GetAIResponse(string prompt)
        {
            var requestBody = new
            {
                model = "phi",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate",
                content
            );

            if (!response.IsSuccessStatusCode)
                return "AI service is currently unavailable.";

            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.TryGetProperty("response", out var reply))
                return reply.GetString() ?? "No response generated.";

            return "AI did not return a valid response.";
        }
    }
}
