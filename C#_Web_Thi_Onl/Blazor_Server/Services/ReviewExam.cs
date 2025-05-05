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
            var getalllerningsumary = await _httpClient.GetFromJsonAsync<List<Learning_Summary>>("/api/Learning_Summary/Get");
            var getallstudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var getallUser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
            var getallstudent_class = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            var getallclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
            var getallpointtye = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
            var getallscorde = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
            var getallSubjet = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var getalltest = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var getallEXrR_packkage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var getallEXrR_student = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var getallEX_history = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_History/Get");
            var getallEXroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var getallQuestion = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var getallAnwer = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");
            var getallhis = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");

            var result = (from 
                          Learning in getalllerningsumary join
                          student in getallstudent on Learning.Student_Id equals student.Id
                          join user in getallUser on student.User_Id equals user.Id
                          join student_class in getallstudent_class on student.Id equals student_class.Student_Id
                          join classs in getallclass on student_class.Class_Id equals classs.Id
                          join score in getallscorde on student.Id equals score.Student_Id
                          join pointtype in getallpointtye on score.Point_Type_Id equals pointtype.Id
                          join packages in getallpackage on pointtype.Id equals packages.Point_Type_Id
                          join test in getalltest on packages.Id equals test.Package_Id
                          join subject in getallSubjet on packages.Subject_Id equals subject.Id
                          join examroompackage in getallEXrR_packkage on packages.Id equals examroompackage.Package_Id
                          join examroom in getallEXroom on examroompackage.Exam_Room_Id equals examroom.Id
                          join examroomstudent in getallEXrR_student on examroompackage.Id equals examroomstudent.Exam_Room_Package_Id  
                          join examhistory in getallEX_history on examroomstudent.Id equals examhistory.Exam_Room_Student_Id
                          join syudenthis in getallhis on examroomstudent.Id equals syudenthis.Exam_Room_Student_Id
                          where student.Student_Code == number && test.Test_Code == codetest
                          select new Review
                          {
                              Student_Name = user.Full_Name,
                              Class_Name = classs.Class_Name,
                              Subject_Name = subject.Subject_Name,
                              Package_Name = packages.Package_Name,
                              Start_Time = ConvertLong.ConvertLongToDateTime(examroom.Start_Time),
                              Start_Time_play =ConvertLong.ConvertLongToDateTime( examroomstudent.Check_Time),
                              End_Time =ConvertLong.ConvertLongToDateTime( examroom.End_Time),
                              End_Time_stop =ConvertLong.ConvertLongToDateTime( examhistory.Create_Time),
                              Score = score.Point,
                              studentAnswers = getallhis
                            .Where(h => h.Exam_Room_Student_Id == examroomstudent.Id)
                            .ToList(),

                              questions = getallQuestion
                                .Where(q => q.Package_Id == packages.Id)
                                .ToList(),
                              answers = getallAnwer
                                .Where(a => getallQuestion
                                    .Where(q => q.Package_Id == packages.Id)
                                    .Select(q => q.Id)
                                    .Contains(a.Question_Id))
                                .ToList()
                          }).ToList();

            return result;
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
