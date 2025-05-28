using Blazor_Server.Pages;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Net.Http;
using static Blazor_Server.Services.ExammanagementService;

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
        public async Task<List<lispackage>> GetAllHistories(int idclass, int idsubject, DateTime start, DateTime end)
        {
            var getallexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var examroom = getallexamroom
                .Where(x =>
                {
                    var roomStart = ConvertLong.ConvertLongToDateTime(x.Start_Time);
                    var roomEnd = ConvertLong.ConvertLongToDateTime(x.End_Time);
                    return roomEnd >= start && roomStart <= end;
                }).ToList();

            if (examroom == null || examroom.Count == 0) return new List<lispackage>();

            var getallexamroompackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var examroompackage = getallexamroompackage
            .Where(x => examroom.Any(y => y.Id == x.Exam_Room_Id))
            .ToList();

            if (examroompackage == null || examroompackage.Count == 0) return new List<lispackage>();

            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var package = getallpackage
                .Where(x => x.Class_Id == idclass && x.Subject_Id == idsubject && examroompackage.Any(y => y.Package_Id == x.Id))
            .ToList();

            if (package == null || package.Count == 0) return new List<lispackage>();

            var getallpackagetype = await _httpClient.GetFromJsonAsync<List<Package_Type>>("/api/Package_Type/Get");

            var result = new List<lispackage>();

            foreach (var pack in package)
            {
                var type = getallpackagetype.FirstOrDefault(pt => pt.Id == pack.Package_Type_Id);
                result.Add(new lispackage
                {
                    Id = pack.Id,
                    Name_package = pack.Package_Name,
                    Name_Package_Type = type?.Package_Type_Name
                });
            }

            return result;
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

                string studentName = "N/A";
                double score = 0;
                DateTime checkTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;
                if (examStudent != null)
                {
                    if (examStudent.Check_Time != null)
                        checkTime = ConvertLong.ConvertLongToDateTime(examStudent.Check_Time);

                    var examHistory = getexamHistories.FirstOrDefault(e => e.Exam_Room_Student_Id == examStudent.Id);
                    if (examHistory != null)
                    {
                        score = examHistory.Score;
                        endTime = ConvertLong.ConvertLongToDateTime(examHistory.Create_Time);
                    }

                    var student = getStudents.FirstOrDefault(s => s.Id == examStudent.Student_Id);
                    if (student != null)
                    {
                        var user = getUsers.FirstOrDefault(u => u.Id == student.User_Id);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Full_Name))
                        {
                            studentName = user.Full_Name;
                        }
                    }
                }

                result.Add(new listTest
                {
                    Id = test.Id,
                    Idpackage = test.Package_Id,
                    Test_Code = test.Test_Code ?? "N/A",
                    Status = test.Status,
                    Name_Student = studentName,
                    score = score,
                    Check_Time = checkTime,
                    End_Time = endTime
                });
            }

            return result;
        }

        public async Task<List<listquestion>> GetQuestions(int testId)
        {
            var testQuestions = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get");
            var questions = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var question_type = await _httpClient.GetFromJsonAsync<List<Question_Type>>("/api/Question_Type/Get");
            var question_lever = await _httpClient.GetFromJsonAsync<List<Question_Level>>("/api/Question_Level/Get");
            var answers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");
            var histories = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
            var relatedTestQuestions = testQuestions.Where(x => x.Test_Id == testId).ToList();

            var result = new List<listquestion>();

            foreach (var tq in relatedTestQuestions)
            {
                var question = questions.FirstOrDefault(q => q.Id == tq.Question_Id);
                if (question == null) continue;

                var relatedAnswers = answers.Where(a => a.Question_Id == question.Id).ToList();
                var type = question_type.FirstOrDefault(x => x.Id == question.Question_Type_Id);
                var lever = question_lever.FirstOrDefault(x => x.Id == question.Question_Level_Id);
                var relatedHistories = histories.Where(h => relatedAnswers.Select(a => a.Id).Contains(h.Answer_Id)).ToList();

                result.Add(new listquestion
                {
                    Id= testId,
                    question_type =type.Package_Type_Id,
                    question_lever=lever.Question_Level_Name,
                    Questions = question,
                    Answers = relatedAnswers,
                    Exam_Room_Student_Answer_HisTories = relatedHistories
                });
            }

            return result;
        }
        public async Task<bool> updateExamHis(int id, double score)
        {
            var getExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var Examroomstudent = getExamRoomStudents.FirstOrDefault(x => x.Test_Id == id);

            if (Examroomstudent == null)
            {
                Console.WriteLine($"Không tìm thấy ExamRoomStudent cho Test_Id = {id}");
                return false;
            }
            var getallexamhis = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");
            var examhis = getallexamhis.FirstOrDefault(x => x.Exam_Room_Student_Id == Examroomstudent.Id);
            if (examhis == null)
            {
                Console.WriteLine($"Không tìm thấy Exam_HisTory cho Exam_Room_Student_Id = {Examroomstudent.Id}");
                return false;
            }
            examhis.Score = score;
            var respon = await _httpClient.PutAsJsonAsync($"/api/Exam_HisTory/Pus/{examhis.Id}", examhis);
            if (!respon.IsSuccessStatusCode)
            {
                Console.WriteLine($"Cập nhật thất bại: {(int)respon.StatusCode} - {respon.ReasonPhrase}");
                return false;
            }
            var getTests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var test = getTests.FirstOrDefault(x => x.Id == id);
            if (test == null)
            {
                Console.WriteLine($"Không tìm thấy test");
                return false;
            }
            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var package = getallpackage.FirstOrDefault(x => x.Id == test.Package_Id);
            if (package == null)
            {
                Console.WriteLine($"Không tìm thấy package");
                return false;
            }
            var getallpointype = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
            var pointype = getallpointype.FirstOrDefault(x => x.Id == package.Point_Type_Id);
            if (pointype == null)
            {
                Console.WriteLine($"Không tìm thấy pointype");
                return false;
            }
            var getallscore =await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
            var allscore = getallscore.FirstOrDefault(x => x.Point_Type_Id == pointype.Id);
            if (allscore == null)
            {
                Console.WriteLine($"Không tìm thấy allscore");
                return false;
            }
            allscore.Point = score;
            var updatescore =await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{allscore.Id}", allscore);
            if (!updatescore.IsSuccessStatusCode)
            {
                Console.WriteLine($"Cập nhật thất bại: {(int)updatescore.StatusCode} - {updatescore.ReasonPhrase}");
                return false;
            }
            return true;
        }

        public class lispackage
        {
            public int Id { get; set; }
            public string Name_package { get; set; }
            public string Name_Package_Type { get; set; }
        }
        public class listTest
        {
            public int Id { get; set; }
            public int Idpackage { get; set; }
            public string Test_Code { get; set; }
            public int Status { get; set; }
            public string Name_Student { get; set; }
            public double score { get; set; }
            public DateTime Check_Time { get; set; }
            public DateTime End_Time { get; set; }
        }
        public class listquestion
        {
            public int Id { get; set; }
            public int question_type { get; set; }
            public string question_lever { get; set; }
            public Question Questions { get; set; } 
            public List<Answers> Answers { get; set; }
            public List<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }
        }
    }
}
