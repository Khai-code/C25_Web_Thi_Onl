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
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static Blazor_Server.Services.HistoriesExam;
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
        public async Task<List<listpackage>> GetallPackage(int id, DateTime? from=null, DateTime? to=null)
        {
            var listPackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get")
                              ?? new List<Data_Base.Models.P.Package>();

            var tasks = listPackage.Select(async package =>
            {
                var listExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get")
                                          ?? new List<Exam_Room_Package>();
                var exrPackage = listExamRoomPackage.FirstOrDefault(x => x.Package_Id == package.Id);
                if (exrPackage == null) return null;

                var listExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get")
                                    ?? new List<Exam_Room>();
                var examRoom = listExamRoom.FirstOrDefault(x => x.Id == exrPackage.Exam_Room_Id && x.Exam_Id == id);
                if (examRoom == null) return null;

                var listExamRoomTeachers = await _httpClient.GetFromJsonAsync<List<Exam_Room_Teacher>>("/api/Exam_Room_Teacher/Get")
                           ?? new List<Exam_Room_Teacher>();

                var proctorIds = listExamRoomTeachers
                    .Where(x => x.Exam_Room_Id == examRoom.Id)
                    .Select(x => x.Teacher_Id)
                    .ToList();

                var startTime = ConvertLong.ConvertLongToDateTime(examRoom.Start_Time);
                if (from != null && startTime < from) return null;
                if (to != null && startTime > to) return null;

                var listRoom = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get") ?? new List<Room>();
                var room = listRoom.FirstOrDefault(x => x.ID == examRoom.Room_Id);
                if (room == null) return null;

                return new listpackage
                {
                    Id = package.Id,
                    Idexam = examRoom.Id,
                    NamePackage = package.Package_Name,
                    StartTime = startTime,
                    EndTime = ConvertLong.ConvertLongToDateTime(examRoom.End_Time),
                    RoomName = room.Room_Name,
                    PackageTypeID = package.Package_Type_Id,
                    TeacherId = package.Teacher_Id,
                    ConfirmTheTest = 0,
                    ProctorTeacherIds = proctorIds,
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

        public async Task<Exam_Room_Student?> GetExamRoomStudentByTestId(int testId)
        {
            var filter = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Test_Id", testId.ToString() }
        }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Exam_Room_Student/common/get", filter);
            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<List<Exam_Room_Student>>();
            return data?.FirstOrDefault();
        }

        public async Task<Exam_HisTory?> GetExamHistoryByExamRoomStudentId(int examRoomStudentId)
        {
            var filter = new CommonFilterRequest
            {
                Filters = new Dictionary<string, string>
        {
            { "Exam_Room_Student_Id", examRoomStudentId.ToString() }
        }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Exam_HisTory/common/get", filter);
            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<List<Exam_HisTory>>();
            return data?.FirstOrDefault();
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


        public async Task<List<listquestion>> GetFullQuestionsByTest(int testId)
        {
            // Lấy danh sách Test_Question theo testId
            var testQuestions = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get");
            var testQuestionsFiltered = testQuestions?
                .Where(x => x.Test_Id == testId)
                .ToList();
            var Review_Test = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
            var reviewTest = Review_Test?.FirstOrDefault(rt => rt.Test_Id == testId)?.Status;
            // Lấy danh sách Question liên quan đến Test
            var questions = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get");
            var questionsFiltered = questions?
                .Where(q => testQuestionsFiltered.Any(tq => tq.Question_Id == q.Id))
                .ToList();

            // Lấy loại câu hỏi
            var question_type = await _httpClient.GetFromJsonAsync<List<Question_Type>>("/api/Question_Type/Get");
            var question_typeFiltered = question_type?
                .Where(qt => questionsFiltered.Any(q => q.Question_Type_Id == qt.Id))
                .ToList();

            // Lấy mức độ câu hỏi
            var question_level = await _httpClient.GetFromJsonAsync<List<Question_Level>>("/api/Question_Level/Get");
            var question_levelFiltered = question_level?
                .Where(ql => questionsFiltered.Any(q => q.Question_Level_Id == ql.Id))
                .ToList();

            // Lấy danh sách học sinh tham gia thi
            var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var examRoomStudentFiltered = examRoomStudents?
                .Where(ers => ers.Test_Id == testId)
                .ToList();

            // Lấy lịch sử trả lời của học sinh
            var histories = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
            var historiesFiltered = histories?
                .Where(h => examRoomStudentFiltered.Any(a => a.Id == h.Exam_Room_Student_Id))
                .ToList();

            // Lấy các đáp án có liên quan tới câu hỏi và nằm trong lịch sử
            var answers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");

            List<Answers> relatedAnswers = new();
            List<Exam_Room_Student_Answer_HisTory> relatedHistories = new();
            // Tạo danh sách kết quả
            var result = new List<listquestion>();

            foreach (var question in questionsFiltered)
            {
                var type = question_typeFiltered?.FirstOrDefault(qt => qt.Id == question.Question_Type_Id);
                var level = question_levelFiltered?.FirstOrDefault(ql => ql.Id == question.Question_Level_Id);
                if (type.Id == 4 || type.Id == 5)
                {
                    var answersFiltered = answers
                .Where(a => historiesFiltered.Any(h => h.Answer_Id == a.Id) &&
                            questionsFiltered.Any(q => q.Id == a.Question_Id))
                .ToList();
                    relatedAnswers = answersFiltered
                     .Where(a => a.Question_Id == question.Id)
                     .ToList();

                    relatedHistories = historiesFiltered
                        .Where(h => relatedAnswers.Any(a => a.Id == h.Answer_Id))
                        .ToList();
                }
                else
                {
                    var answersFiltered = answers?
                .Where(a => questionsFiltered.Any(q => q.Id == a.Question_Id))
                .ToList();
                    relatedAnswers = answersFiltered
                        .Where(a => a.Question_Id == question.Id)
                        .ToList();
                    relatedHistories = historiesFiltered
                        .Where(h => relatedAnswers.Any(a => a.Id == h.Answer_Id))
                        .ToList();
                }

                result.Add(new listquestion
                {
                    Id = question.Id,
                    statuss = reviewTest,
                    question_type = type?.Package_Type_Id ?? 0,
                    question_lever = level?.Question_Level_Name ?? "",
                    Questions = question,
                    Answers = relatedAnswers,
                    Exam_Room_Student_Answer_HisTories = relatedHistories
                });
            }

            return result;
        }


        public async Task<bool> SaveAllScores(int testId, List<Answers> updatedAnswers)
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

            var examRoomStudent = await GetExamRoomStudentByTestId(testId);
            if (examRoomStudent == null) return false;

            // Bước 2: Lấy duy nhất Exam_HisTory theo ExamRoomStudentId
            var examHistory = await GetExamHistoryByExamRoomStudentId(examRoomStudent.Id);
            if (examHistory == null) return false;

            if (examHistory != null)
            {
                examHistory.Score = totalScore;
                var updateHistoryResponse = await _httpClient.PutAsJsonAsync($"/api/Exam_HisTory/Pus/{examHistory.Id}", examHistory);
            }

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
                Summary_Id = summaryId,
                Test_Id = testId
            };

            var checkScore = await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{lstScore.Id}", score);
            if (!checkScore.IsSuccessStatusCode)
                return false;

            var testResp = await _httpClient.GetAsync($"/api/Test/GetBy/{testId}");
            if (testResp.IsSuccessStatusCode)
            {
                var test = await testResp.Content.ReadFromJsonAsync<Data_Base.Models.T.Test>();
                if (test != null)
                {
                    test.Status = 1;
                    var updateTestResp = await _httpClient.PutAsJsonAsync($"/api/Test/Pus/{testId}", test);
       
                    if (!updateTestResp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Cập nhật status cho Test {testId} thất bại.");
                    }
                }
            }

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

        public async Task<bool> ConfirmExamRoom(int examRoomId, int teacherId)
        {
            try
            {
                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
            {
                { "Exam_Room_Id", examRoomId.ToString() },
                { "Teacher_Id", teacherId.ToString() }
            }
                };

                // Lấy bản ghi Exam_Room_Teacher hiện tại
                var response = await _httpClient.PostAsJsonAsync("/api/Exam_Room_Teacher/common/get", filter);
                if (!response.IsSuccessStatusCode) return false;

                var list = await response.Content.ReadFromJsonAsync<List<Exam_Room_Teacher>>();
                var examRoomTeacher = list?.FirstOrDefault();
                if (examRoomTeacher == null) return false;

                examRoomTeacher.Confirm_The_Test = 1;

                var updateResp = await _httpClient.PutAsJsonAsync($"/api/Exam_Room_Teacher/Pus/{examRoomTeacher.Id}", examRoomTeacher);
                return updateResp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xác nhận bài thi: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Room>> GetAllRooms()
        {
            var rooms = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get");
            return rooms ?? new List<Room>();
        }

        public async Task<bool> AddRoom(Room room)
        {
            try
            {
                // Tối thiểu cần Room_Name
                if (room == null || string.IsNullOrWhiteSpace(room.Room_Name))
                    return false;

                // (Optional) Kiểm tra trùng tên phòng trước khi tạo
                var exists = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get") ?? new List<Room>();
                if (exists.Any(r => string.Equals(r.Room_Name?.Trim(), room.Room_Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    // Nếu muốn coi trùng tên là lỗi, return false; hoặc thêm hậu tố tự động
                    return false;
                }

                // Gọi API tạo mới
                var resp = await _httpClient.PostAsJsonAsync("/api/Room/Post", room);
                if (!resp.IsSuccessStatusCode) return false;

                // Nếu API trả về object Room mới, bạn có thể đọc về (không bắt buộc)
                // var created = await resp.Content.ReadFromJsonAsync<Room>();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddRoom] Lỗi: {ex.Message}");
                return false;
            }
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
            public int TeacherId { get; set; } // giáo viên ra đề
            public int ConfirmTheTest { get; set; }

            public List<int> ProctorTeacherIds { get; set; } = new();

        }
        public class listStudent
        {
            public int Id { get; set; }
            public int packagecode { get; set; }
            public string NameStudent { get; set; }
            public string status { get; set; }
        }
        public class ExamModel
        {
            public string ExamName { get; set; }
            public int SubjectId { get; set; }
        }

    }
}
