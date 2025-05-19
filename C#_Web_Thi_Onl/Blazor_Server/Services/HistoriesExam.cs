using Blazor_Server.Pages;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Net.Http;

namespace Blazor_Server.Services
{
    public class HistoriesExam
    {
        private readonly HttpClient _httpClient;
        public HistoriesExam(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<List<Class>> getallclasses()
        {
            return _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
        }
        public Task<List<Subject>> getallsubjects()
        {
            return _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
        }
        public async Task<List<Data_Base.Models.P.Package>> GetAllHistories(int idclass, int idsubject, DateTime start, DateTime end)
        {
            var getallexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var examroom = getallexamroom
             .Where(x =>
             {
                 var roomStart = ConvertLong.ConvertLongToDateTime(x.Start_Time);
                 var roomEnd = ConvertLong.ConvertLongToDateTime(x.End_Time);

                 return roomEnd >= start && roomStart <= end;
             }).ToList();

            if (examroom == null) return null;
            var getallexamroompackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var examroompackage = getallexamroompackage.Where(x => examroom.Select(y => y.Id).Contains(x.Exam_Room_Id)).ToList();
            if (examroompackage == null) return null;
            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var package = getallpackage.Where(x => x.Class_Id == idclass && x.Subject_Id == idsubject && examroompackage.Select(y=>y.Package_Id).Contains(x.Id)).ToList();
            return package;
            
        }

        public async Task<List<listTest>> GetTests(int packageId)
        {
            var getTests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var getExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var getStudents = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var getUsers = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
            var getexamHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");
            var filteredTests = getTests.Where(t => t.Package_Id == packageId).ToList();
            var result = new List<listTest>();
            foreach (var test in filteredTests)
            {
                var examStudent = getExamRoomStudents.FirstOrDefault(e => e.Test_Id == test.Id);
                string studentName = null;
                double score = 0;
                DateTime endTime=DateTime.Now;
                if (examStudent != null) 
                {
                    
                    var examHistory = getexamHistories.FirstOrDefault(e => e.Exam_Room_Student_Id == examStudent.Id);
                    if (examHistory != null)
                    {
                        score = examHistory.Score;
                        endTime =ConvertLong.ConvertLongToDateTime( examHistory.Create_Time);
                    }
                    var student = getStudents.FirstOrDefault(s => s.Id == examStudent.Student_Id);
                    if (student != null)
                    {
                        var user = getUsers.FirstOrDefault(u => u.Id == student.User_Id);
                        if (user != null)
                        {
                            studentName = user.Full_Name;
                        }
                    }
                }

                result.Add(new listTest
                {
                    Id = test.Id,
                    Test_Code = test.Test_Code,
                    Status = test.Status,
                    Name_Student = studentName,
                    score = score,
                    Check_Time = ConvertLong.ConvertLongToDateTime(examStudent.Check_Time),
                    End_Time = endTime
                });
            }

            return result;
        }
        public async Task<List<listquestion>> GetQuestions(int testId)
        {
            var testQuestions = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get");
            var questions = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var answers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");
            var histories = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
            var relatedTestQuestions = testQuestions.Where(x => x.Test_Id == testId).ToList();

            var result = new List<listquestion>();

            foreach (var tq in relatedTestQuestions)
            {
                var question = questions.FirstOrDefault(q => q.Id == tq.Question_Id);
                if (question == null) continue;

                var relatedAnswers = answers.Where(a => a.Question_Id == question.Id).ToList();
                var relatedHistories = histories.Where(h => relatedAnswers.Select(a => a.Id).Contains(h.Answer_Id)).ToList();

                result.Add(new listquestion
                {
                    Questions = question,
                    Answers = relatedAnswers,
                    Exam_Room_Student_Answer_HisTories = relatedHistories
                });
            }

            return result;
        }

        public class listTest
        {
            public int Id { get; set; }
            public string Test_Code { get; set; }
            public int Status { get; set; }
            public string Name_Student { get; set; }
            public double score { get; set; }
            public DateTime Check_Time { get; set; }
            public DateTime End_Time { get; set; }
        }
        public class listquestion
        {
            public Question Questions { get; set; } 
            public List<Answers> Answers { get; set; }
            public List<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }
        }
    }
}
