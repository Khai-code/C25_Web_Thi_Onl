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

        public async Task<Question> AddQuestionWithAnswersAsync(QuestionViewModel model)
        {
            var maxAnswers = GetMaxAnswersForType(model.Question.Type);
            if (model.Answers.Count > maxAnswers)
            {
                throw new ArgumentException($"Số lượng đáp án không hợp lệ cho loại câu hỏi này. Tối đa là {maxAnswers} đáp án.");
            }

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/Post", model.Question);

            if (response.IsSuccessStatusCode)
            {
                var createdQuestion = await response.Content.ReadFromJsonAsync<Question>();

                // Sau khi tạo câu hỏi, tạo các đáp án
                foreach (var answer in model.Answers)
                {
                    answer.Question_Id = createdQuestion.Id;
                    await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/Post", answer);
                }

                return createdQuestion;
            }

            return null;
        }

        private int GetMaxAnswersForType(int type)
        {
            if (type == 1) return 10; // Vòng 1: Chọn nhiều đáp án
            if (type == 2) return 4;  // Vòng 2: Chọn 4 đáp án
            if (type == 3) return 2;  // Vòng 3: Chỉ có True/False
            return 0;
        }

        public class QuestionViewModel
        {
            public Question Question { get; set; }
            public List<Answers> Answers { get; set; }
        }
    }
}
