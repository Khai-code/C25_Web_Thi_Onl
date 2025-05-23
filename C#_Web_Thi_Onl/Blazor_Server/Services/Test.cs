using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;

namespace Blazor_Server.Services
{
    public class Test
    {
        private readonly HttpClient _httpClient;
        public Test(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HistDTO> GetQuestionAnswers(int Package_Code)
        {
            if (Package_Code == null)
            {
                return null;
            }

            #region vế 1: lấy test
            var packages = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Package/Get");

            var packageId = packages.FirstOrDefault(o => o.Package_Code == Package_Code)?.Id;

            if (packageId == null)
            {
                return null;
            }

            var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("https://localhost:7187/api/Test/Get");

            var test = tests.Where(o => o.Package_Id == packageId && o.Status == 0).ToList();

            if (tests == null)
            {
                return null;
            }

            var random = new Random();
            var randomTest = test[random.Next(test.Count)];
            #endregion

            #region vế 2: lấy Questions và Answers
            var Questions_Pack = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.Q.Question>>("https://localhost:7187/api/Package/Get");

            var Questions = Questions_Pack.Where(o => o.Package_Id == packageId).ToList();

            if (Questions == null)
            {
                return null;
            }

            var QuestionId = Questions.Select(o => o.Id).ToList();

            var Answers_Questions = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.A.Answers>>("https://localhost:7187/api/Answers/Get");

            var Answers = Answers_Questions.Where(o => QuestionId.Contains(o.Question_Id)).ToList();

            if (Answers == null)
            {
                return null;
            }
            #endregion

            #region vế 3: lấy ra bài thì câu hỏi và đáp án tương ứng
            var hist = new HistDTO
            {
                TestId = randomTest.Id,
                Code = randomTest.Test_Code,
                Status = randomTest.Status,
                Questions = Questions.Select(Que => new Question
                {
                    QuestionId = Que.Id,
                    Type = Que.Question_Type_Id,
                    Answers = Answers.Select(Ans => new Answer
                    {
                        AnswerId = Ans.Id,
                        AnswersName = Ans.Answers_Name
                    }).ToList(),
                }).ToList()
            };
            #endregion

            return hist;
        }

        public async Task<List<Exam_Room_Student_Answer_HisTory>> CreateHis(int Package_Code, int StudentId, int QuestIonId, List<int> AnswerIds)
        {
            if (Package_Code == 0 || AnswerIds == null || AnswerIds.Count == 0)
            {
                return null;
            }

            #region vế 1: lấy ExamRoomPackages và ExamRoomStudent
            var packages = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Package/Get");

            var packageId = packages.FirstOrDefault(o => o.Package_Code == Package_Code)?.Id;

            if (packageId == null)
            {
                return null;
            }

            var ExamRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");

            var ExamRoomPackageId = ExamRoomPackages.FirstOrDefault(o => o.Package_Id == packageId)?.Id;

            if (ExamRoomPackageId == null)
            {
                return null;
            }

            var ExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var ExamRoomStudent = ExamRoomStudents.FirstOrDefault(o => o.Exam_Room_Package_Id == ExamRoomPackageId).Id;
            #endregion

            #region vế 2: khởi tạo và lưu đáp án đã chọn (Exam_Room_Student_Answer_HisTory(), xóa nếu đã có
            var AnsHislst = new List<Exam_Room_Student_Answer_HisTory>();

            var Answers = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.A.Answers>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var AnswersId = Answers.Where(o => o.Question_Id == QuestIonId).Select(x => x.Id).ToList();

            if (AnswersId == null)
            {
                return null;
            }

            var SAHs = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var SAH = SAHs.Where(o => AnswersId.Contains(o.ID) && o.Exam_Room_Student_Id == ExamRoomStudent).ToList();
            if (SAH != null || SAH.Count > 0)
            {
                foreach (var item in SAH)
                {
                    await _httpClient.DeleteAsync($"https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/Delete/{item.ID}");
                }
            }
            else
            {
                foreach (var answerId in AnswerIds)
                {
                    var ansHis = new Exam_Room_Student_Answer_HisTory
                    {
                        Answer_Id = answerId,
                        Exam_Room_Student_Id = ExamRoomStudent
                    };
                    var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/Post", ansHis);

                    if (response.IsSuccessStatusCode)
                    {
                        var ERSAH = await response.Content.ReadFromJsonAsync<Exam_Room_Student_Answer_HisTory>();
                        AnsHislst.Add(ERSAH);
                    }
                }
            }
            #endregion

            return AnsHislst;
        }

        public class HistDTO
        {
            public int TestId { get; set; }
            public string Code { get; set; }
            public int Status { get; set; }
            public List<Question> Questions { get; set; }
        }

        public class Question
        {
            public int QuestionId { get; set; }
            public int Type { get; set; }
            public string QuestionName { get; set; }
            public List<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public int AnswerId { get; set; }
            public string AnswersName { get; set; }
        }
    }
}
