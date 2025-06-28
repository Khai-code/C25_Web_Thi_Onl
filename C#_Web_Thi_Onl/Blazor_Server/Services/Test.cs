using Data_Base.Filters;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using static Blazor_Server.Services.HistoriesExam;

namespace Blazor_Server.Services
{
    public class Test
    {
        private readonly HttpClient _httpClient;
        public Test(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HistDTO> GetQuestionAnswers(int Package_Code, string Student_Code)
        {
            if (Package_Code == null)
            {
                return null;
            }

            #region vế 1: lấy test
            var filterRequestPackage = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
                    {
                        { "Package_Code", Package_Code.ToString() }
                    },
            };
            var packageGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Package_/common/get", filterRequestPackage);

            if (packageGetResponse.IsSuccessStatusCode)
                return null;

            var Vpackage = (await packageGetResponse.Content.ReadFromJsonAsync<List<Data_Base.V_Model.V_Package>>()).SingleOrDefault();

            if (Vpackage == null)
                return null;

            var filterRequestStudent = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
                    {
                        { "Student_Code", Student_Code.ToString() }
                    },
            };
            var studentGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Student_/common/get", filterRequestPackage);

            if (studentGetResponse.IsSuccessStatusCode)
                return null;

            var student = (await packageGetResponse.Content.ReadFromJsonAsync<List<Data_Base.V_Model.V_Student>>()).SingleOrDefault();

            if (student == null)
                return null;

            var filterRequestTest = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", Vpackage.Id.ToString() },
                        { "Student_Id", student.Id.ToString() }
                    },
            };
            var lstTestsReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/common/get", filterRequestTest);

            var test = (await lstTestsReq.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test>>()).SingleOrDefault();

            #endregion

            #region vế 2: lấy Questions và Answers
            var filterRequestQuestion = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", Vpackage.Id.ToString() }
                    },
            };

            var questionsReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filterRequestQuestion);

            var lstQuestions = await questionsReq.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();

            if (lstQuestions == null)
            {
                return null;
            }

            var randomQuestionIds = lstQuestions.OrderBy(id => Guid.NewGuid()) // trộn ngẫu nhiên danh sách ID
                                    .Take(Vpackage.Number_Of_Questions) // lấy đúng số lượng mong muốn
                                    .ToList();

            List<Data_Base.Models.T.Test_Question> lstTq = new List<Test_Question>();
            
            foreach (var question in randomQuestionIds)
            {
                Test_Question tq = new Test_Question();
                tq.Question_Id = question.Id;
                tq.Test_Id = test.Id;

                lstTq.Add(tq);
            }

            if (lstTq != null && lstTq.Count > 0)
            {
                await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test_Question/PostList", lstTq);
            }

            var filterRequestAnswers = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
                    {
                        { "Question_Id", string.Join(",", randomQuestionIds.Select(o => o.Id)) }
                    },
            };

            var answersReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filterRequestAnswers);

            if (answersReq == null)
                return null;

            var lstAnswers = await answersReq.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();

            #endregion

            #region vế 3: lấy ra bài thì câu hỏi và đáp án tương ứng
            var hist = new HistDTO // package
            {
                PackageId = Vpackage.Id,
                PackageName = Vpackage.Package_Name,
                PackageType = Vpackage.Package_Type_Name,
                ExecutionTime = Vpackage.ExecutionTime,
                SubjectName = Vpackage.Subject_Name,
                ClassName = Vpackage.Class_Name,
                PointTypeId = Vpackage.Point_Type_Id,
                TestId = test.Id,
                TestCode = test.Test_Code,
                Status = test.Status,
                Questions = randomQuestionIds.Select(Que => new Question
                {
                    QuestionId = Que.Id,
                    Type = Que.Question_Type_Id,
                    Level = Que.Question_Level_Id,
                    Answers = lstAnswers.Select(Ans => new Answer
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
            public int PackageId { get; set; }
            public string PackageName { get; set; }
            public string PackageType { get; set; }
            public int ExecutionTime { get; set; }
            public int TestId { get; set; }
            public string TestCode { get; set; }
            public string SubjectName { get; set; }
            public string ClassName { get; set; }
            public int PointTypeId { get; set; }
            public int Status { get; set; }
            public List<Question> Questions { get; set; }
        }

        public class Question
        {
            public int? QuestionId { get; set; }
            public string QuestionName { get; set; }
            public int Type { get; set; }
            public int Level { get; set; }
            public List<Answer> Answers { get; set; }
        }

        public class Answer
        {
            public int? AnswerId { get; set; }
            public string AnswersName { get; set; }
            public int IsCorrect { get; set; }
        }
    }
}
