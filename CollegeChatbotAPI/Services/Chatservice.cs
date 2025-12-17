namespace CollegeChatbotAPI.Services
{
    public class ChatService
    {
        private readonly IAIService _aiService;

        public ChatService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<string> GetResponse(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return "Please enter a valid question.";
           
            var prompt = $@"
You are a college enquiry assistant.

You MUST answer questions related to:
- admissions
- courses
- fees
- departments
- facilities
- hostel
- contact details

Admissions questions ARE allowed and expected.

If the question is NOT related to a college, reply:
'I can only answer questions related to this college.'

Answer clearly and directly.

User question: {userMessage}
";


            return await _aiService.GetAIResponse(prompt);
        }
    }
}
