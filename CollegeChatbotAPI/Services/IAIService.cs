namespace CollegeChatbotAPI.Services
{
    public interface IAIService
    {
        Task<string> GetAIResponse(string prompt);   
    }
}
