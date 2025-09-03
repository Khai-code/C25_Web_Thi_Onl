using Data_Base.Models.A;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.GenericRepositories;

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
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<PackageViewModel>> GetPackage()
        {
            try
            {
                var lstPackage = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Package/Get");
                if (lstPackage == null || lstPackage.Count == 0) return new List<PackageViewModel>();

                var lstExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");
                if (lstExamRoomPackage == null || lstExamRoomPackage.Count == 0) return new List<PackageViewModel>();

                var lstExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Exam_Room/Get");
                if (lstExamRoom == null || lstExamRoom.Count == 0) return new List<PackageViewModel>();

                DateTime now = DateTime.Now;
                long DateTimeNow = ConvertLong.ConvertDateTimeToLong(now);

                var result = (from package in lstPackage
                              join examRoomPackage in lstExamRoomPackage on package.Id equals examRoomPackage.Package_Id
                              join examRoom in lstExamRoom on examRoomPackage.Exam_Room_Id equals examRoom.Id
                              where examRoom.Start_Time > DateTimeNow
                              select new PackageViewModel
                              {
                                  Package_Id = package.Id,
                                  Package_Name = package.Package_Name
                              }).Distinct().ToList();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<PackageQuestionSDO>> PackageQuestion()
        {
            try
            {
                var lstPackage = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Package/Get");
                if (lstPackage == null || lstPackage.Count == 0) return new List<PackageQuestionSDO>();

                var lstExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");
                if (lstExamRoomPackage == null || lstExamRoomPackage.Count == 0) return new List<PackageQuestionSDO>();

                var lstExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Exam_Room/Get");
                if (lstExamRoom == null || lstExamRoom.Count == 0) return new List<PackageQuestionSDO>();

                var lstQuestion = await _httpClient.GetFromJsonAsync<List<Question>>("https://localhost:7187/api/Question/Get");
                if (lstQuestion == null || lstQuestion.Count == 0) return new List<PackageQuestionSDO>();

                var lstAnswers = await _httpClient.GetFromJsonAsync<List<Answers>>("https://localhost:7187/api/Answers/Get");
                if (lstAnswers == null || lstAnswers.Count == 0) return new List<PackageQuestionSDO>();

                DateTime now = DateTime.Now;
                long DateTimeNow = ConvertLong.ConvertDateTimeToLong(now);

                var filteredPackages = (from package in lstPackage
                                        join examRoomPackage in lstExamRoomPackage on package.Id equals examRoomPackage.Package_Id
                                        join examRoom in lstExamRoom on examRoomPackage.Exam_Room_Id equals examRoom.Id
                                        where examRoom.End_Time < DateTimeNow
                                        select package).Distinct().ToList();

                var result = (from package in filteredPackages
                              join question in lstQuestion on package.Id equals question.Package_Id
                              select new PackageQuestionSDO
                              {
                                  Package_Name = package.Package_Name,
                                  Question_Name = question.Question_Name,
                                  lstAnswers = lstAnswers.Where(a => a.Question_Id == question.Id).ToList()
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class PackageQuestionSDO
        {
            public string Package_Name { get; set; }
            public string Question_Name { get; set; }
            public List<Answers> lstAnswers { get; set; }
        }

        public class PackageViewModel
        {
            public int Package_Id { get; set; }
            public string Package_Name { get; set; }
        }

        public class QuestionViewModel
        {
            public Question Question { get; set; }
            public List<Answers> Answers { get; set; }
        }
    }
}
