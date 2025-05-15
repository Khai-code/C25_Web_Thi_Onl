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
            var getalllsumary = await _httpClient.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
            var currentTime = DateTime.Now;
            var sumary = getalllsumary.FirstOrDefault(x => currentTime >= ConvertLong.ConvertLongToDateTime(x.Start_Time) && currentTime <= ConvertLong.ConvertLongToDateTime(x.End_Time));
            if (sumary == null) return null;
            var getalllerningsumary = await _httpClient.GetFromJsonAsync<List<Learning_Summary>>("/api/Learning_Summary/Get");
            var lerningsumary = getalllerningsumary.FirstOrDefault(x => x.Summary_ID == sumary.Id);
            if (lerningsumary == null) return null;
            var getallstudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var student = getallstudent.FirstOrDefault(x => x.Id == lerningsumary.Student_Id && x.Student_Code == number);
            if (student == null) return null;
            var getallUser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
            var user = getallUser.FirstOrDefault(x => x.Id == student.User_Id);
            if (user == null) return null;
            var getallstudent_class = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            var student_class = getallstudent_class.FirstOrDefault(x => x.Student_Id == student.Id);
            if (student_class == null) return null;
            var getallclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
            var classs = getallclass.FirstOrDefault(x => x.Id == student_class.Class_Id);
            if (classs == null) return null;
            var getallscorde = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
            var scorde = getallscorde.FirstOrDefault(x => x.Student_Id == student.Id);
            if (scorde == null) return null;
            var getallpointtye = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
            var pointtye = getallpointtye.FirstOrDefault(x => x.Id == scorde.Point_Type_Id);
            if (pointtye == null) return null;
            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var package = getallpackage.FirstOrDefault(x => x.Point_Type_Id == pointtye.Id);
            if (package == null) return null;
            var getalltest = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var test = getalltest.FirstOrDefault(x => x.Package_Id == package.Id && x.Test_Code == codetest);
            if (test == null) return null;
            var getallSubjet = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            var subject = getallSubjet.FirstOrDefault(x => x.Id == package.Subject_Id);
            if (subject == null) return null;
            var getallEXrR_packkage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var exrpakage = getallEXrR_packkage.FirstOrDefault(x => x.Package_Id == package.Id);
            if (exrpakage == null) return null;
            var getallEXroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var examroom = getallEXroom.FirstOrDefault(x => x.Id == exrpakage.Exam_Room_Id);
            if (examroom == null) return null;
            var getallEXrR_student = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var exrstudent = getallEXrR_student.FirstOrDefault(x => x.Exam_Room_Package_Id == exrpakage.Id);
            if (exrstudent == null) return null;
            var getallEX_history = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_History/Get");
            var now = DateTime.Now;
            var exhistory = getallEX_history.FirstOrDefault(x =>
            {
                var finishTime = ConvertLong.ConvertLongToDateTime(x.Create_Time);
                return x.Exam_Room_Student_Id == exrstudent.Id &&
                       (now - finishTime).TotalDays < 7;
            });
            if (exhistory == null) return null;
            var getallQuestion = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var getallAnwer = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");
            var getallExamRoomStudentAnswerHisTory = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
            var result = new List<Review>();
            result.Add( new Review
                          {
                              Student_Name = user.Full_Name,
                              Class_Name = classs.Class_Name,
                              Subject_Name = subject.Subject_Name,
                              Package_Name = package.Package_Name,
                              Start_Time = ConvertLong.ConvertLongToDateTime(examroom.Start_Time),
                              Start_Time_play =ConvertLong.ConvertLongToDateTime(exrstudent.Check_Time),
                              End_Time =ConvertLong.ConvertLongToDateTime( examroom.End_Time),
                              End_Time_stop =ConvertLong.ConvertLongToDateTime(exhistory.Create_Time),
                              Score = exhistory.Score,
                              studentAnswers = getallExamRoomStudentAnswerHisTory
                            .Where(h => h.Exam_Room_Student_Id == exrstudent.Id)
                            .ToList(),
                              questions = getallQuestion
                                .Where(q => q.Package_Id == package.Id)
                                .ToList(),
                              answers = getallAnwer
                                .Where(a => getallQuestion
                                    .Where(q => q.Package_Id == package.Id)
                                    .Select(q => q.Id)
                                    .Contains(a.Question_Id))
                                .ToList()
                          });

            return result.ToList();
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
