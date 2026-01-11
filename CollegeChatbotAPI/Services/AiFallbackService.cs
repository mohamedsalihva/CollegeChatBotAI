using CollegeChatbotAPI.DTOs;

namespace CollegeChatbotAPI.Services
{
    public class AiFallbackService
    {
        private readonly IAIService _aiService;

        public AiFallbackService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<ChatResponse> GetFallbackResponse(string userMessage)
        {
            string prompt = $@"
You are a college enquiry assistant.
Answer only college related questions.

User question:
{userMessage}
";

            var aiAnswer = await _aiService.GetAIResponse(prompt);

            return new ChatResponse
            {
                Answer = aiAnswer,
                Source = "AI"
            };
        }
    }
}
