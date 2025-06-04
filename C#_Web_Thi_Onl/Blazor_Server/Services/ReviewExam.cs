using Blazor_Server.Pages;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.L;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Data;
using static Blazor_Server.Services.ExammanagementService;


namespace Blazor_Server.Services
{

    public class ReviewExam
    {
        private readonly HttpClient _httpClient;
        public ReviewExam(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Summary>> GetAllSumary()
        {
            var data = await _httpClient.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
            return data;
        }
        public async Task<List<Review>> SeacherReview(string number, string codetest)
        {
            var now = DateTime.Now;
            var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var student = students.FirstOrDefault(x => x.Student_Code == number);
            if (student == null) return null;
            var users = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
            var user = users.FirstOrDefault(x => x.Id == student.User_Id);
            if (user == null) return null;
            var scores = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
            var studentScores = scores.Where(x => x.Student_Id == student.Id).ToList();
            if (!studentScores.Any()) return null;
            var pointTypes = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
            var relatedPointTypes = pointTypes.Where(x => studentScores.Select(s => s.Point_Type_Id).Contains(x.Id)).ToList();
            if (!relatedPointTypes.Any()) return null;
            var summaries = await _httpClient.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
            var activeSummaries = summaries
                .Where(x => relatedPointTypes.Select(p => p.Summary_Id).Contains(x.Id)
                            && now >= ConvertLong.ConvertLongToDateTime(x.Start_Time)
                            && now <= ConvertLong.ConvertLongToDateTime(x.End_Time))
                .ToList();
            if (!activeSummaries.Any()) return null;
            var studentClasses = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            var studentClass = studentClasses.FirstOrDefault(x => x.Student_Id == student.Id);
            if (studentClass == null) return null;
            var classes = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
            var studentClassObj = classes.FirstOrDefault(x => x.Id == studentClass.Class_Id);
            if (studentClassObj == null) return null;
            var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var relatedPackages = packages.Where(p => relatedPointTypes.Select(pt => pt.Id).Contains(p.Point_Type_Id)).ToList();
            if (!relatedPackages.Any()) return null;
            var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var test = tests.FirstOrDefault(t => t.Test_Code == codetest && relatedPackages.Select(p => p.Id).Contains(t.Package_Id));
            if (test == null) return null;
            var selectedPackage = relatedPackages.FirstOrDefault(p => p.Id == test.Package_Id);
            if (selectedPackage == null) return null;
            var subjects = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            var subject = subjects.FirstOrDefault(s => s.Id == selectedPackage.Subject_Id);
            if (subject == null) return null;
            var examRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var selectedExamRoomPackages = examRoomPackages.Where(x => x.Package_Id == selectedPackage.Id).ToList();
            if (!selectedExamRoomPackages.Any()) return null;
            var examRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var examRoom = examRooms.FirstOrDefault(x => selectedExamRoomPackages.Select(p => p.Exam_Room_Id).Contains(x.Id));
            if (examRoom == null) return null;
            var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var examRoomStudent = examRoomStudents.FirstOrDefault(x =>
                selectedExamRoomPackages.Select(p => p.Id).Contains(x.Exam_Room_Package_Id) &&
                x.Student_Id == student.Id);
            if (examRoomStudent == null) return null;
            var getalltest_question = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get");
            var test_question = getalltest_question.Where(x => x.Test_Id == test.Id).ToList();
            if (test_question == null) return null;
            var examHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_History/Get");
            var recentExamHistory = examHistories.FirstOrDefault(x =>
                x.Exam_Room_Student_Id == examRoomStudent.Id &&
                (now - ConvertLong.ConvertLongToDateTime(x.Create_Time)).TotalDays < 7);
            if (recentExamHistory == null) return null;
            var questions = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var packageQuestions = questions.Where(q => test_question.Select(s => s.Question_Id).Contains(q.Id)).ToList();
            var answers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");
            var questionIds = packageQuestions.Select(q => q.Id).ToList();
            var relatedAnswers = answers.Where(a => questionIds.Contains(a.Question_Id)).ToList();
            var answerHistories = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
            var studentAnswerHistories = answerHistories.Where(h => h.Exam_Room_Student_Id == examRoomStudent.Id).ToList();
            return new List<Review>
             {
                 new Review
                 {
                     idtest=test.Id,
                     Student_Name = user.Full_Name,
                     Class_Name = studentClassObj.Class_Name,
                     Subject_Name = subject.Subject_Name,
                     Package_Name = selectedPackage.Package_Name,
                     Start_Time = ConvertLong.ConvertLongToDateTime(examRoom.Start_Time),
                     Start_Time_play = ConvertLong.ConvertLongToDateTime(examRoomStudent.Check_Time),
                     End_Time = ConvertLong.ConvertLongToDateTime(examRoom.End_Time),
                     End_Time_stop = ConvertLong.ConvertLongToDateTime(recentExamHistory.Create_Time),
                     Score = recentExamHistory.Score,
                     studentAnswers = studentAnswerHistories,
                     questions = packageQuestions,
                     answers = relatedAnswers
                 }
             };
        }

        public async Task UpdateReview(int id, int rightAnswer)
        {
            var data = await _httpClient.GetFromJsonAsync<Answers>($"/api/Answers/GetBy/{id}");
            var answer = new Answers
            {
                Id = data.Id,
                Answers_Name=data.Answers_Name,
                Question_Id=data.Question_Id,
                Right_Answer = rightAnswer
            };
            var response = await _httpClient.PutAsJsonAsync($"/api/Answers/Pus/{id}", answer);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Cập nhật thành công cho đáp án ID: {answer.Id}");
            }
            else
            {
                Console.WriteLine($"Cập nhật thất bại cho đáp án ID: {answer.Id}");
            }
        }

        public class Review
        {
            public int idtest { get; set; }
            public string Student_Name { get; set; }
            public string Class_Name { get; set; }
            public string Subject_Name { get; set; }
            public string Package_Name { get; set; }
            public DateTime Start_Time { get; set; }
            public DateTime Start_Time_play { get; set; }
            public DateTime End_Time { get; set; }
            public DateTime End_Time_stop { get; set; }
            public double Score { get; set; }
            public List<Exam_Room_Student_Answer_HisTory> studentAnswers { get; set; }
            public List<Question> questions { get; set; }
            public List<Answers> answers { get; set; }
        }
    }
}
