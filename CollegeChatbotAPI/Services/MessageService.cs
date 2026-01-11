namespace CollegeChatbotAPI.Services
{
    public class MessageService
    {
        public string Normalize(string message)
        {
            return message
                .ToLower()
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "")
                .Replace("!", "")
                .Trim();
        }
    }
}
