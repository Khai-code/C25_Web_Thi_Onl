using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
namespace Blazor_Server.Services
{
    public class ExammanagementService
    {
        private readonly HttpClient _httpClient;
        public ExammanagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> cancelexam(int id, int packagecode)
        {
            var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
            var package = getallpackage.FirstOrDefault(x => x.Package_Code == packagecode);
            if (package == null)
                return false;
            var getalltest = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
            var test = getalltest.FirstOrDefault(x => x.Package_Id == package.Id && x.Student_Id == id);
            if (test == null)
                return false;
            var getallexamroomstudent = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var examRoomStudent = getallexamroomstudent.FirstOrDefault(x => x.Test_Id == test.Id && x.Student_Id == id);
            var ressutl = new Exam_HisTory
            {
                Exam_Room_Student_Id = examRoomStudent.Id,
                Create_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
                Actual_Execution_Time = 0,
                Score = 0
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Exam_HisTory/Post", ressutl);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Không thể thêm lịch sử thi.");
                return false;
            }
            return true;

        }
        public async Task<List<listexam>> SeachExam(DateTime start, DateTime end)
        {
            var result = new List<listexam>();
            var listExam = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new List<Exam>();
            var examRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var roomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
            foreach (var exam in listExam)
            {
                // Lọc các phòng thi thuộc bài thi hiện tại và có thời gian bắt đầu hoặc kết thúc trong khoảng start-end
                var validRooms = examRooms
                    .Where(x => x.Exam_Id == exam.Id)
                    .Where(x =>
                    {
                        var roomStart = ConvertLong.ConvertLongToDateTime(x.Start_Time);
                        var roomEnd = ConvertLong.ConvertLongToDateTime(x.End_Time);
                        // Chỉnh lại điều kiện: phòng thi có bắt đầu hoặc kết thúc trong khoảng start - end
                        //return (roomStart >= start && roomStart <= end) || (roomEnd >= start && roomEnd <= end);
                        return roomStart <= end && roomEnd >= start;
                    })
                    .ToList();

                if (!validRooms.Any()) continue;

                int totalPackage = 0;

                foreach (var room in validRooms)
                {
                    var rpk = roomPackages.Where(x => x.Exam_Room_Id == room.Id);
                    foreach (var pkg in rpk)
                    {
                        if (packages.Any(x => x.Id == pkg.Package_Id))
                        {
                            totalPackage++;
                        }
                    }
                }

                result.Add(new listexam
                {
                    Id = exam.Id,
                    NameExam = exam.Exam_Name,
                    Totalpackage = totalPackage
                });
            }

            return result;
        }

        public async Task<List<listexam>> GetallExam()
        {
            var result = new List<listexam>();
            var ListExam = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new List<Exam>();
            foreach(var exam in ListExam)
            {
                var examroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
                var valiTime = examroom.Where(x=>x.Exam_Id==exam.Id).ToList();
                //if (!valiTime.Any()) continue;
                int total = 0;
                foreach(var room in valiTime)
                {
                    var roomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
                    var rpk = roomPackages.Where(x => x.Exam_Room_Id == room.Id);
                    foreach(var pakage in rpk)
                    {
                        var package = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
                        var list = package.Where(x => x.Id == pakage.Package_Id);
                        if (list != null) total++;
                    }
                }
                result.Add(new listexam
                {
                    Id = exam.Id,
                    NameExam = exam.Exam_Name,
                    Totalpackage = total
                });
            }
            return result;
        }
        public async Task<List<listpackage>> GetallPackage(int id)
        {
            var listPackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();

            var tasks = listPackage.Select(async package =>
            {
                var listExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
                var exrPackage = listExamRoomPackage.FirstOrDefault(x => x.Package_Id == package.Id);
                if (exrPackage == null) return null;
                var listExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get")?? new List<Exam_Room>();
                var examRoom = listExamRoom.FirstOrDefault(x =>x.Id == exrPackage.Exam_Room_Id &&x.Exam_Id == id && IsOverlapCurrentWeek(x.Start_Time, x.End_Time));
                if (examRoom == null) return null;
                var listRoom = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get")?? new List<Room>();
                var room = listRoom.FirstOrDefault(x => x.ID == examRoom.Room_Id);
                if (room == null) return null;
                return new listpackage
                {
                    Id = package.Id,
                    Idexam = examRoom.Id,
                    NamePackage = package.Package_Name,
                    StartTime = ConvertLong.ConvertLongToDateTime(examRoom.Start_Time),
                    EndTime = ConvertLong.ConvertLongToDateTime(examRoom.End_Time),
                    RoomName = room.Room_Name,
                    PackageTypeID = package.Package_Type_Id
                };
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(x => x != null).Distinct().ToList();

        }

        public async Task<List<listStudent>> GetAllStudent(int Id)
        {
            var Listpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
            var package = Listpackage.FirstOrDefault(x => x.Id == Id);
            if (package == null) return new List<listStudent>();
            var Lisclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get") ?? new List<Class>();
            var classpackage = Lisclass.FirstOrDefault(x => x.Id == package.Class_Id);
            if (classpackage == null) return new List<listStudent>();
            var Liststdclass = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get") ?? new List<Student_Class>();
            var studentClasses = Liststdclass.Where(x => x.Class_Id == classpackage.Id).ToList();
            if (!studentClasses.Any()) return new List<listStudent>();
            var Liststudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new List<Student>();
            var Listuser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new List<User>();
            var Listexroompk = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var expk = Listexroompk.FirstOrDefault(x => x.Package_Id == package.Id);
            if (expk == null) return new List<listStudent>();
            var Listexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var exroom = Listexamroom.FirstOrDefault(x => x.Id == expk.Exam_Room_Id);
            if (exroom == null) return new List<listStudent>();
            var Listexroomstudent = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new List<Exam_Room_Student>();
            var Listexhistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get") ?? new List<Exam_HisTory>();
            var result = new List<listStudent>();
            foreach (var studentClass in studentClasses)
            {
                var student = Liststudent.FirstOrDefault(s => s.Id == studentClass.Student_Id);
                if (student == null) continue;

                var user = Listuser.FirstOrDefault(u => u.Id == student.User_Id);
                if (user == null) continue;

                var exstd = Listexroomstudent.FirstOrDefault(x => x.Exam_Room_Package_Id == expk.Id && x.Student_Id == studentClass.Student_Id);

                string status = "Chưa thi";

                if (exstd != null)
                {
                    var startTime = ConvertLong.ConvertLongToDateTime(exroom.Start_Time);
                    var endTime = ConvertLong.ConvertLongToDateTime(exroom.End_Time);
                    var checkTime = ConvertLong.ConvertLongToDateTime(exstd.Check_Time);

                    var studentHistories = Listexhistories
                        .Where(h => h.Exam_Room_Student_Id == exstd.Id)
                        .ToList();

                    if (DateTime.Now < startTime)
                    {
                        status = "Chưa thi";
                    }
                    else if (studentHistories.Any(h => h.Actual_Execution_Time == 0))
                    {
                        var cheatTime = studentHistories.First(h => h.Actual_Execution_Time == 0).Create_Time;
                        var cheatDateTime = ConvertLong.ConvertLongToDateTime(cheatTime);
                        status = $"Đã phát hiện gian lận lúc {cheatDateTime:HH:mm:ss dd/MM/yyyy}";
                    }
                    else if (studentHistories.Any(h =>
                     ConvertLong.ConvertLongToDateTime(h.Create_Time) >= startTime &&
                     ConvertLong.ConvertLongToDateTime(h.Create_Time) <= endTime))
                    {
                        var finishHistory = studentHistories.First(h =>
                            ConvertLong.ConvertLongToDateTime(h.Create_Time) >= startTime &&
                            ConvertLong.ConvertLongToDateTime(h.Create_Time) <= endTime);

                        var finishTime = ConvertLong.ConvertLongToDateTime(finishHistory.Create_Time);
                        status = $"Đã hoàn thành bài thi lúc {finishTime:HH:mm:ss dd/MM/yyyy}";
                    }

                    else if (checkTime >= startTime && checkTime < endTime)
                    {
                        status = $"Đang thi lúc{checkTime:HH:mm:ss dd/MM/yyyy}";
                    }
                    else
                    {
                        status = "Đã thi";
                    }
                }


                result.Add(new listStudent
                {
                    Id = student.Id,
                    NameStudent = user.Full_Name,
                    packagecode = package.Package_Code,
                    status = status
                });
            }

            return result.ToList();
        }

        public async Task<bool> UpdateExamRoomTime(int id, Exam_Room exam_Room)
        {
            var existingExamRoom = await _httpClient.GetFromJsonAsync<Exam_Room>($"/api/Exam_Room/GetBy/{id}");

            if (existingExamRoom == null)
            {
                Console.WriteLine($" ID {id} không tồn tại.");
                return false;
            }
            existingExamRoom.Start_Time = exam_Room.Start_Time ;
            existingExamRoom.End_Time = exam_Room.End_Time;
            var response = await _httpClient.PutAsJsonAsync($"/api/Exam_Room/Pus/{id}", existingExamRoom);
            return response.IsSuccessStatusCode;
        }
       
        private bool IsOverlapCurrentWeek(long startTime, long endTime)
        {
            DateTime today = DateTime.Today;
            int diff = today.DayOfWeek == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - today.DayOfWeek;
            DateTime startOfWeek = today.AddDays(diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);

            DateTime roomStartTime = ConvertLong.ConvertLongToDateTime(startTime);
            DateTime roomEndTime = ConvertLong.ConvertLongToDateTime(endTime);
            if (roomStartTime < startOfWeek || roomStartTime > endOfWeek)
            {
                return false;
            }

            // Chỉ cần thời gian thi có GIAO với tuần hiện tại là tính
            return roomStartTime <= endOfWeek && roomEndTime >= startOfWeek;
           
        }


        public async Task<Exam> AddExam(Exam exam)
        {
            try
            {
                var addexam = await _httpClient.PostAsJsonAsync("/api/Exam/Post", exam);

                if (!addexam.IsSuccessStatusCode)
                    return null;

                // Giải mã nội dung từ phản hồi thành đối tượng Exam
                var examResult = await addexam.Content.ReadFromJsonAsync<Exam>();

                return examResult; // Trả về đối tượng Exam đã giải mã
            }
            catch (Exception ex)
            {
                // Có thể ghi log exception nếu cần
                throw new ApplicationException("Có lỗi xảy ra khi thêm kỳ thi.", ex);
            }
        }

        public async Task<List<Data_Base.Models.T.Test>> GetTestsByPackage(int packageId)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Package_Id", packageId.ToString() }
        }
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Test/common/get", request);
            if (!response.IsSuccessStatusCode)
                return new List<Data_Base.Models.T.Test>();
            var tests = await response.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test>>();
            return tests ?? new List<Data_Base.Models.T.Test>();
        }

        public async Task<List<Test_Question>> GetTestQuestionsByTest(int testId)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Test_Id", testId.ToString() }
        }
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Test_Question/common/get", request);
            if (!response.IsSuccessStatusCode)
                return new List<Test_Question>();
            var testQuestions = await response.Content.ReadFromJsonAsync<List<Test_Question>>();
            return testQuestions ?? new List<Test_Question>();
        }

        public async Task<List<Question>> GetQuestionsByIds(List<int> questionIds)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Id", string.Join(",", questionIds) }
        }
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Question/common/get", request);
            if (!response.IsSuccessStatusCode)
                return new List<Question>();
            var questions = await response.Content.ReadFromJsonAsync<List<Question>>();
            return questions ?? new List<Question>();
        }

        public async Task<List<Answers>> GetAnswersByQuestion(int questionId)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Question_Id", questionId.ToString() }
        }
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Answers/common/get", request);
            if (!response.IsSuccessStatusCode)
                return new List<Answers>();
            var answers = await response.Content.ReadFromJsonAsync<List<Answers>>();
            return answers ?? new List<Answers>();
        }

        // Lấy lịch sử thi theo Exam_Room_Student_Id
        public async Task<Exam_HisTory?> GetExamHistoryByExamRoomStudentId(int examRoomStudentId)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Exam_Room_Student_Id", examRoomStudentId.ToString() }
        }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Exam_HisTory/common/get", request);
            if (!response.IsSuccessStatusCode) return null;

            var histories = await response.Content.ReadFromJsonAsync<List<Exam_HisTory>>();
            return histories?.SingleOrDefault();
        }



        public async Task<List<Exam_Room_Student>> GetExamRoomStudentsByPackageId(int packageId)
        {
            var request = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Exam_Room_Package_Id", packageId.ToString() }
        }
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Exam_Room_Student/common/get", request);
            if (!response.IsSuccessStatusCode)
                return new List<Exam_Room_Student>();
            var data = await response.Content.ReadFromJsonAsync<List<Exam_Room_Student>>();
            return data ?? new List<Exam_Room_Student>();
        }


        public async Task<List<QuestionWithAnswers>> GetFullQuestionsByTest(int testId)
        {
            var testQuestions = await GetTestQuestionsByTest(testId);
            var questionIds = testQuestions.Select(q => q.Question_Id).ToList();
            var questions = await GetQuestionsByIds(questionIds);

            var questionWithAnswers = new List<QuestionWithAnswers>();
            foreach (var question in questions)
            {
                var answers = await GetAnswersByQuestion(question.Id);
                questionWithAnswers.Add(new QuestionWithAnswers
                {
                    Question = question,
                    Answers = answers
                });
            }
            return questionWithAnswers;
        }


        public async Task<bool> SaveAllScores(int examRoomStudentId, List<Answers> updatedAnswers)
        {
            // 1. Update từng Answer
            double totalScore = 0;
            foreach (var answer in updatedAnswers)
            {
                // Update Answer trong db
                var answerResponse = await _httpClient.GetAsync($"/api/Answers/GetBy/{answer.Id}");
                if (!answerResponse.IsSuccessStatusCode) continue;
                var answerDb = await answerResponse.Content.ReadFromJsonAsync<Answers>();
                if (answerDb == null) continue;
                answerDb.Points_Earned = answer.Points_Earned;
                await _httpClient.PutAsJsonAsync($"/api/Answers/Pus/{answer.Id}", answerDb);

                // Cộng tổng điểm
                totalScore += answer.Points_Earned ?? 0;
            }

            // 2. Update lại Score trong Exam_HisTory
            var examHistory = await GetExamHistoryByExamRoomStudentId(examRoomStudentId);
            if (examHistory != null)
            {
                examHistory.Score = totalScore;
                var updateHistoryResponse = await _httpClient.PutAsJsonAsync($"/api/Exam_HisTory/Pus/{examHistory.Id}", examHistory);
            }

            // 3. Cập nhật vào bảng Score (chỉ update, không tạo mới)
            // -- Lấy thông tin liên quan
            var examRoomStudent = await _httpClient.GetFromJsonAsync<Exam_Room_Student>($"/api/Exam_Room_Student/GetBy/{examRoomStudentId}");
            if (examRoomStudent == null) return false;

            int studentId = examRoomStudent.Student_Id;
            var examRoomPackage = await _httpClient.GetFromJsonAsync<Exam_Room_Package>($"/api/Exam_Room_Package/GetBy/{examRoomStudent.Exam_Room_Package_Id}");
            if (examRoomPackage == null) return false;

            var package = await _httpClient.GetFromJsonAsync<Data_Base.Models.P.Package>($"/api/Package/GetBy/{examRoomPackage.Package_Id}");
            if (package == null) return false;

            int subjectId = package.Subject_Id;
            int pointTypeId = package.Package_Type_Id;

            // -- Xác định SummaryId hiện tại (kỳ thi đang diễn ra)
            int summaryId = await GetCurrentSummaryId();

            var filterScore = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Student_Id", studentId.ToString() },
            { "Subject_Id", subjectId.ToString() },
            { "Point_Type_Id", pointTypeId.ToString() },
            { "Summary_Id", summaryId.ToString() }
        }
            };

            var scoreRep = await _httpClient.PostAsJsonAsync("/api/Score/common/get", filterScore);
            if (!scoreRep.IsSuccessStatusCode)
                return false;

            // Lấy bản ghi đầu tiên chưa có điểm (Point == 0)
            var lstScore = (await scoreRep.Content.ReadFromJsonAsync<List<Score>>()).Where(s => s.Point == 0).FirstOrDefault();
            if (lstScore == null)
                return true; // Không còn bản ghi trống để update, coi như đã xong

            // Update điểm
            Data_Base.Models.S.Score score = new Score
            {
                Id = lstScore.Id,
                Student_Id = studentId,
                Subject_Id = subjectId,
                Point_Type_Id = pointTypeId,
                Point = totalScore,
                Summary_Id = summaryId
            };

            var checkScore = await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{lstScore.Id}", score);
            if (!checkScore.IsSuccessStatusCode)
                return false;

            return true;

        }

        public async Task<int> GetCurrentSummaryId()
        {
            var summaries = await _httpClient.GetFromJsonAsync<List<Summary>>("/api/Summary/Get") ?? new();
            var currentTime = DateTime.Now;

            // Tìm kỳ học mà thời gian hiện tại nằm trong phạm vi
            var today = DateTime.Now.Date;

            var currentSummary = summaries.FirstOrDefault(s =>
                today >= ConvertLong.ConvertLongToDateTime(s.Start_Time).Date &&
                today <= ConvertLong.ConvertLongToDateTime(s.End_Time).Date);

            return currentSummary?.Id ?? 0;
        }

        public class QuestionWithAnswers
        {
            public Question Question { get; set; }
            public List<Answers> Answers { get; set; }
        }

        public class listexam
        {
            public int Id { get; set; }
            public string NameExam { get; set; }
            public int Totalpackage { get; set; } 
        }
        public class posthis
        {
            public int Exam_Room_Student_Id { get; set; }
            public long Create_Time { get; set; }
            public int Actual_Execution_Time { get; set; }
            public float Score { get; set; }
        }
        public class listpackage 
        {
            public int Id { get; set; }
            public int Idexam { get; set; }
            public string NamePackage { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string RoomName {  get; set; }
            public int PackageTypeID { get; set; }

        }
        public class listStudent
        {
            public int Id { get; set; }
            public int packagecode { get; set; }
            public string NameStudent { get; set; }
            public string status { get; set; }
        }
    }
}
