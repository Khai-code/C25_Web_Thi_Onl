using Data_Base.Models.C;
using Data_Base.Models.L;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;

namespace Blazor_Server.Services
{
    public class Learning_SummaryService
    {
        private HttpClient _client;

        public Learning_SummaryService(HttpClient client)
        {
            _client = client;
        }

        // Lấy tất cả các lớp
        public async Task<List<Class>> GetAllClass()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Class>>("/api/Class/Get");
                return response ?? new List<Class>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllClass: {ex.Message}");
                return new List<Class>();
            }
        }

        public async Task<List<Teacher>> GetTeachers()
        {
            var response = await _client.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get");
            return response ?? new List<Teacher>();
        }


        public async Task<List<ClassWithTeacherName>> GetClassesWithTeacherName()
        {
            var classes = await GetAllClass();
            var teachers = await GetTeachers();
            var users = await GetUsers();

            var result = classes.Select(cls =>
            {
                var teacher = teachers.FirstOrDefault(t => t.Id == cls.Teacher_Id);
                var user = users.FirstOrDefault(u => u.Id == teacher?.User_Id);
                return new ClassWithTeacherName
                {
                    Id = cls.Id,
                    Class_Name = cls.Class_Name,
                    Teacher_Id = cls.Teacher_Id,
                    Numbers = cls.Number,
                    TeacherName = user?.Full_Name ?? "[Không rõ]"
                };
            }).ToList();

            return result;
        }


        // Lấy thông tin các lớp học sinh thuộc
        public async Task<List<Student_Class>> GetStudent_Classes(int classId)
        {
            var response = await _client.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            return response.Where(sc => sc.Class_Id == classId).ToList();
        }

        // Lấy điểm của học sinh theo danh sách ID học sinh
        public async Task<List<Score>> GetScoresByStudent(List<int> studentIds)
        {
            var response = await _client.GetFromJsonAsync<List<Score>>("/api/Score/Get");
            return response.Where(s => studentIds.Contains(s.Student_Id)).ToList();
        }

        // Lấy thông tin các loại điểm (Attendance, Midterm, ...)
        public async Task<List<Point_Type>> GetPoint_Types()
        {
            var response = await _client.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
            return response ?? new List<Point_Type>();
        }

        // Lấy thông tin tất cả học sinh
        public async Task<List<Student>> GetStudents()
        {
            var response = await _client.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            return response ?? new List<Student>();
        }

        // Lấy thông tin các môn học
        public async Task<List<Subject>> GetSubjects()
        {
            var response = await _client.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            return response ?? new List<Subject>();
        }

        // Lấy thông tin người dùng (học sinh, giáo viên...)
        public async Task<List<User>> GetUsers()
        {
            var response = await _client.GetFromJsonAsync<List<User>>("/api/User/Get");
            return response ?? new List<User>();
        }

        // Lấy thông tin kỳ học (Summary)
        public async Task<Summary> GetCurrentSummary()
        {
            try
            {
                var currentTime = DateTime.Now;
                long currentTimeLong = currentTime.Year * 10000000000 + currentTime.Month * 100000000 + currentTime.Day * 1000000 +
                                      currentTime.Hour * 10000 + currentTime.Minute * 100 + currentTime.Second;

                var summaries = await _client.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
                var current = summaries.FirstOrDefault(s => currentTimeLong >= s.Start_Time && currentTimeLong <= s.End_Time);

                return current; // sẽ trả null nếu không nằm trong kỳ nào
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCurrentSummary: {ex.Message}");
                return null;
            }
        }

        //public async Task<List<Learning_SummaryView>> CalculateLearningSummaryOnly(int classId)
        //{
        //    var studentClass = await GetStudent_Classes(classId);
        //    var studentIds = studentClass.Select(sc => sc.Student_Id).Distinct().ToList();

        //    var scores = await GetScoresByStudent(studentIds);
        //    var pointTypes = await GetPoint_Types();
        //    var students = await GetStudents();
        //    var subjects = await GetSubjects();
        //    var users = await GetUsers();

        //    var allPointTypes = await GetPoint_Types();
        //    var currentSummary = await GetCurrentSummary();

        //    var pointTypess = allPointTypes
        //        .Where(p => p.Summary_Id == currentSummary?.Id)
        //        .ToList();

        //    var pointTypeMap = pointTypess.ToDictionary(p => p.Point_Type_Name, p => p.Id);
        //    var result = new List<Learning_SummaryView>();

        //    foreach (var studentId in studentIds)
        //    {
        //        var studentScore = scores.Where(s => s.Student_Id == studentId).ToList();
        //        var groupBySubject = studentScore.GroupBy(s => s.Subject_Id);

        //        foreach (var group in groupBySubject)
        //        {
        //            var subjectId = group.Key;
        //            var scoreBySubject = group.ToList();

        //            double avg(string name) =>
        //                pointTypeMap.ContainsKey(name) ?
        //                scoreBySubject.Where(s => s.Point_Type_Id == pointTypeMap[name]).Select(s => s.Point).DefaultIfEmpty(0).Average() : 0;

        //            double attendance = avg("Attendance");
        //            double p15 = avg("Point_15");
        //            double p45 = avg("Point_45");
        //            double mid = avg("Point_Midterm");
        //            double final = avg("Point_Final");
        //            double summary = (attendance + p15 + p45 + mid + final) / 10;

        //            var student = students.FirstOrDefault(s => s.Id == studentId);
        //            var user = users.FirstOrDefault(u => u.Id == student?.User_Id);
        //            var subject = subjects.FirstOrDefault(s => s.Id == subjectId);

        //            result.Add(new Learning_SummaryView
        //            {
        //                StudentId = studentId,
        //                SubjectId = subjectId,
        //                ClassId = classId,
        //                Student_Name = user?.Full_Name ?? "[Không rõ]",
        //                Subject_Name = subject?.Subject_Name,
        //                Attendance = attendance,
        //                Point_15 = p15,
        //                Point_45 = p45,
        //                Point_Midterm = mid,
        //                Point_Final = final,
        //                Point_Summary = summary,
        //                Summary_ID = currentSummary?.Id ?? 0,
        //                Summary_Name = currentSummary?.Summary_Name ?? ""
        //            });
        //        }
        //    }

        //    return result;
        //}

        // ✅ Hàm lưu vào DB nếu chưa có, cập nhật nếu đã tồn tại
        public async Task SaveCalculatedSummariesToDatabase(List<Learning_SummaryView> summaries)
        {
            var allDbSummaries = await _client.GetFromJsonAsync<List<Learning_SummaryView>>("/api/Learning_Summary/Get");

            foreach (var view in summaries)
            {
                var existing = allDbSummaries?.FirstOrDefault(ls =>
                    ls.StudentId == view.StudentId &&
                    ls.SubjectId == view.SubjectId &&
                    ls.ClassId == view.ClassId);

                if (existing == null)
                {
                    var create = new Learning_Summary
                    {
                        Student_Id = view.StudentId,
                        Subject_Id = view.SubjectId,
                        Attendance = view.Attendance,
                        Point_15 = view.Point_15,
                        Point_45 = view.Point_45,
                        Point_Midterm = view.Point_Midterm,
                        Point_Final = view.Point_Final,
                        Point_Summary = view.Point_Summary,
                        Summary_ID = view.Summary_ID
                    };

                    await _client.PostAsJsonAsync("api/Learning_Summary/Post", create);
                }
                else
                {
                    var update = new Learning_Summary
                    {
                        Id = existing.Id,
                        Student_Id = view.StudentId,
                        Subject_Id = view.SubjectId,
                        Attendance = view.Attendance,
                        Point_15 = view.Point_15,
                        Point_45 = view.Point_45,
                        Point_Midterm = view.Point_Midterm,
                        Point_Final = view.Point_Final,
                        Point_Summary = view.Point_Summary,
                        Summary_ID = view.Summary_ID
                    };

                    await _client.PutAsJsonAsync($"/api/Learning_Summary/Pus/{existing.Id}", update);
                }
            }
        }
        public async Task<List<Learning_SummaryView>> GetAnnualAverageScores(int classId)
        {
            var studentClasses = await _client.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            var students = await _client.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var users = await _client.GetFromJsonAsync<List<User>>("/api/User/Get");
            var subjects = await _client.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            var summaries = await _client.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
            var allScores = await _client.GetFromJsonAsync<List<Learning_Summary>>("/api/Learning_Summary/Get");

            var summary1 = summaries.FirstOrDefault(s => s.Summary_Name.Contains("1"));
            var summary2 = summaries.FirstOrDefault(s => s.Summary_Name.Contains("2"));
            if (summary1 == null || summary2 == null) return new List<Learning_SummaryView>();

            var studentIds = studentClasses
                .Where(sc => sc.Class_Id == classId)
                .Select(sc => sc.Student_Id)
                .ToList();

            var result = new List<Learning_SummaryView>();

            foreach (var studentId in studentIds)
            {
                var student = students.FirstOrDefault(s => s.Id == studentId);
                var user = users.FirstOrDefault(u => u.Id == student?.User_Id);
                var studentName = user?.Full_Name ?? "[Không rõ]";

                // Lấy điểm của kỳ 1 và kỳ 2
                var s1 = allScores
                    .Where(s => s.Student_Id == studentId && s.Summary_ID == summary1.Id)
                    .ToList();

                var s2 = allScores
                    .Where(s => s.Student_Id == studentId && s.Summary_ID == summary2.Id)
                    .ToList();

                // Gom các môn
                var allSubjects = s1.Select(x => x.Subject_Id)
                    .Union(s2.Select(x => x.Subject_Id))
                    .Distinct();

                foreach (var subjectId in allSubjects)
                {
                    var subjectName = subjects.FirstOrDefault(s => s.Id == subjectId)?.Subject_Name ?? "???";

                    var score1 = s1.FirstOrDefault(x => x.Subject_Id == subjectId)?.Point_Summary ?? 0;
                    var score2 = s2.FirstOrDefault(x => x.Subject_Id == subjectId)?.Point_Summary ?? 0;

                    var avg = Math.Round((score1 + score2 * 2) / 3);

                    result.Add(new Learning_SummaryView
                    {
                        StudentId = studentId,
                        Student_Name = studentName,
                        Subject_Name = subjectName,
                        Point_Summary = avg
                    });
                }
            }

            return result;
        }


    }

    public class Learning_SummaryView
    {
        public int Id { get; set; }
        public double Attendance { get; set; }
        public double Point_15 { get; set; }
        public double Point_45 { get; set; }
        public double Point_Midterm { get; set; }
        public double Point_Final { get; set; }
        public double Point_Summary { get; set; }
        public string Student_Name { get; set; }
        public string Subject_Name { get; set; }
        public int Summary_ID { get; set; }
        public string Summary_Name { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public Dictionary<string, double> SubjectScores { get; set; }

        public double Term_Summary { get; set; }
    }

    public class ClassWithTeacherName
    {
        public int Id { get; set; }
        public string Class_Name { get; set; }
        public int Teacher_Id { get; set; }
        public string TeacherName { get; set; }
        public int Numbers { get; set; }
    }

}
