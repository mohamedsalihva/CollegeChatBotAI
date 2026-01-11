using CollegeChatbotAPI.DTOs;

namespace CollegeChatbotAPI.Services
{
    public class FaqService
    {
        private readonly DatabaseService _dbService;

        public FaqService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ChatResponse?> HandleFaqQuery(string msg)
        {
            var matchedFaqs = await _dbService.GetMatchedFaqs(msg);

            if (matchedFaqs.Count == 1)
            {
                var answer = await _dbService.GetFaqAnswerByFaqId(matchedFaqs[0].FaqId);
                return new ChatResponse
                {
                    Answer = answer ?? "Answer not available.",
                    Source = "DATABASE"
                };
            }

            if (matchedFaqs.Count > 1)
            {
                var categories = matchedFaqs.Select(x => x.Category).Distinct();
                return new ChatResponse
                {
                    Answer = "I can help you with " + string.Join(", ", categories) +
                             ". Please ask one topic at a time.",
                    Source = "GUIDANCE"
                };
            }

            return null;
        }
    }
}
