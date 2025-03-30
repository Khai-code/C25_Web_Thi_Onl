using Data_Base.Models.A;
using Data_Base.Models.Q;

namespace Blazor_Server.Services
{
    public class ExamService
    {
        private readonly HttpClient _httpClient;

        public ExamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QuestionViewModel> AddQuestionWithAnswersAsync(QuestionViewModel model)
        {
            // 1️⃣ Gửi yêu cầu thêm câu hỏi
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/Post", model.Question);

            if (!response.IsSuccessStatusCode)
                return null;

            var addedQuestion = await response.Content.ReadFromJsonAsync<Question>();
            if (addedQuestion == null) return null;

            // 2️⃣ Gán Question_Id cho tất cả các đáp án
            foreach (var answer in model.Answers)
            {
                answer.Question_Id = addedQuestion.Id;
            }

            // 3️⃣ Gửi danh sách đáp án lên API /api/Answer/Post
            var answerResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answer/Post", model.Answers);

            if (!answerResponse.IsSuccessStatusCode)
                return null;

            return new QuestionViewModel
            {
                Question = addedQuestion,
                Answers = model.Answers
            };
        }


        public class QuestionViewModel
        {
            public Question Question { get; set; }
            public List<Answers> Answers { get; set; }
        }
    }
}
