using Blazor_Server.Pages;
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
using Data_Base.V_Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using static Blazor_Server.Services.ExammanagementService;
using static Blazor_Server.Services.PackageManager;

namespace Blazor_Server.Services
{
    public class HistoriesExam
    {
        private readonly HttpClient _httpClient;
        public string Error {  get; set; }
        public double doubleScore { get; set; }
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
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<listTest>> GetTests(int packageId)
        {
            try
            {
                var getallTests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var gettest = getallTests?.Where(x => x.Package_Id == packageId).ToList();
                if (gettest == null || !gettest.Any()) return new();

                var getallExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
                var getExamRoomStudents = getallExamRoomStudents?
                    .Where(x => gettest.Any(t => t.Id == x.Test_Id) && x.Is_Check_Review == 0)
                    .ToList();
                if (getExamRoomStudents == null || !getExamRoomStudents.Any()) return new();

                var getallStudents = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
                var getStudents = getallStudents?
                    .Where(x => getExamRoomStudents.Any(e => e.Student_Id == x.Id))
                    .ToList();
                if (getStudents == null || !getStudents.Any()) return new();

                var getallUsers = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
                var getusers = getallUsers?
                    .Where(x => getStudents.Any(s => s.User_Id == x.Id))
                    .ToList();
                if (getusers == null || !getusers.Any()) return new();

                var getallexamHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");
                var getexamHistories = getallexamHistories?
                    .Where(x => getExamRoomStudents.Any(e => e.Id == x.Exam_Room_Student_Id))
                    .ToList();
                if (getexamHistories == null || !getexamHistories.Any()) return new();
                var getallReviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
                var getreviewTests = getallReviewTests?
                    .Where(x => gettest.Any(t => t.Id == x.Test_Id&& x.Teacher_Id==null))
                    .ToList();
                var result = new List<listTest>();

                foreach (var test in gettest)
                {
                    var examRoomStudents = getExamRoomStudents.Where(e => e.Test_Id == test.Id).ToList();
                    foreach (var examRoomStudent in examRoomStudents)
                    {
                        var student = getStudents.FirstOrDefault(s => s.Id == examRoomStudent.Student_Id);
                        var user = getusers.FirstOrDefault(u => u.Id == student?.User_Id);
                        var examHistory = getexamHistories.FirstOrDefault(h => h.Exam_Room_Student_Id == examRoomStudent.Id);
                        var getallScore = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
                        var scoreRecord = getallScore?.FirstOrDefault(s =>
                            s.Student_Id == student.Id && s.Test_Id == test.Id);
                        var reviewTest = await Task.Run(()=> getreviewTests?.FirstOrDefault(rt => rt.Test_Id == test.Id && rt.Student_Id == student?.Id));

                        result.Add(new listTest
                        {
                            Id = test.Id,
                            IdReview = reviewTest?.Id ?? 0,
                            comenteacher = reviewTest?.Reason_For_Refusal ?? "không có thông tin",
                            statustest = reviewTest?.Status ?? 0,
                            comenttest = reviewTest?.Reason_For_Sending ?? "không có thông tin",
                            Idpackage = test.Package_Id,
                            Test_Code = test.Test_Code ?? "N/A",
                            Status = test.Status,
                            Name_Student = user?.Full_Name ?? "Không rõ",
                            score = examHistory?.Score ?? 0,
                            Point = scoreRecord?.Point ?? 0,
                            Check_Time = ConvertLong.ConvertLongToDateTime(examRoomStudent.Check_Time),
                            End_Time = ConvertLong.ConvertLongToDateTime(examHistory.Create_Time),
                            Teacher_Id = reviewTest?.Teacher_Id ?? 0,
                            studnetid = student.Id,
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<listquestion>> GetQuestions(int testId)
        {
            try
            {
                // Lấy danh sách Test_Question theo testId
                var testQuestions = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get");
                var testQuestionsFiltered = testQuestions?
                    .Where(x => x.Test_Id == testId)
                    .ToList();
                var Review_Test = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
                var reviewTest = Review_Test?.FirstOrDefault(rt => rt.Test_Id == testId)?.Status;
                var reviewTestTeacherID = Review_Test?.FirstOrDefault(rt => rt.Test_Id == testId)?.Teacher_Id;
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
                        Exam_Room_Student_Answer_HisTories = relatedHistories,
                        Teacher_Id = reviewTestTeacherID
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> updateExamHis(int id, double score)
        {
            try
            {
                var getExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
                var examRoomStudentList = getExamRoomStudents.Where(x => x.Test_Id == id).ToList();

                if (!examRoomStudentList.Any())
                {
                    Console.WriteLine($"Không tìm thấy ExamRoomStudent cho Test_Id = {id}");
                    return false;
                }

                var getallexamhis = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");
                var examHisList = getallexamhis
                    .Where(x => examRoomStudentList.Any(s => s.Id == x.Exam_Room_Student_Id))
                    .ToList();

                if (!examHisList.Any())
                {
                    Console.WriteLine($"Không tìm thấy Exam_HisTory");
                    return false;
                }

                foreach (var examhis in examHisList)
                {
                    examhis.Score = score;
                    var respon = await _httpClient.PutAsJsonAsync($"/api/Exam_HisTory/Pus/{examhis.Id}", examhis);
                    if (!respon.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Cập nhật Exam_HisTory thất bại: {(int)respon.StatusCode} - {respon.ReasonPhrase}");
                        return false;
                    }
                }

                var getTests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var testList = getTests.Where(x => x.Id == id).ToList();

                if (!testList.Any())
                {
                    Console.WriteLine("Không tìm thấy test");
                    return false;
                }

                var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
                var packageList = getallpackage
                    .Where(p => testList.Any(t => t.Package_Id == p.Id))
                    .ToList();

                if (!packageList.Any())
                {
                    Console.WriteLine("Không tìm thấy package");
                    return false;
                }

                var getallsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
                var subjectList = getallsubject
                    .Where(s => packageList.Any(p => p.Subject_Id == s.Id))
                    .ToList();

                if (!subjectList.Any())
                {
                    Console.WriteLine("Không tìm thấy subject");
                    return false;
                }

                var getallpointtype_subject = await _httpClient.GetFromJsonAsync<List<Point_Type_Subject>>("/api/Point_Type_Subject/Get");
                var ptsList = getallpointtype_subject
                    .Where(pts => subjectList.Any(s => s.Id == pts.Subject_Id))
                    .ToList();

                if (!ptsList.Any())
                {
                    Console.WriteLine("Không tìm thấy pointtype_subject");
                    return false;
                }

                var getallpointype = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
                var pointypeList = getallpointype
                    .Where(pt => ptsList.Any(pts => pts.Point_Type_Id == pt.Id))
                    .ToList();

                if (!pointypeList.Any())
                {
                    Console.WriteLine("Không tìm thấy pointype");
                    return false;
                }

                var getallscore = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get");
                var scoreList = getallscore
                    .Where(s => pointypeList.Any(pt => pt.Id == s.Point_Type_Id) && subjectList.Any(sj => sj.Id == s.Subject_Id) && examRoomStudentList.Any(exstd => exstd.Student_Id == s.Student_Id))
                    .ToList();

                if (!scoreList.Any())
                {
                    Console.WriteLine("Không tìm thấy allscore");
                    return false;
                }
                foreach (var s in scoreList)
                {
                    s.Point = score;
                    var updatescore = await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{s.Id}", s);
                    if (!updatescore.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Cập nhật điểm thất bại: {(int)updatescore.StatusCode} - {updatescore.ReasonPhrase}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> SaveAllScoresByTestId(int testId, double totalScore)
        {
            try
            {
                // Lấy Score với TestId
                var filterScore = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                {
                    { "Test_Id", testId.ToString() }
                }
                };
                var scoreRep = await _httpClient.PostAsJsonAsync("/api/Score/common/get", filterScore);
                if (!scoreRep.IsSuccessStatusCode) return false;

                var scoreToUpdate = (await scoreRep.Content.ReadFromJsonAsync<List<Score>>()).SingleOrDefault();
                if (scoreToUpdate == null) return false;

                scoreToUpdate.Point = totalScore;
                var updateScoreResp = await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{scoreToUpdate.Id}", scoreToUpdate);

                return updateScoreResp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> updatecomemt(int id, string? text)
        {
            try
            {
                var existing = await _httpClient.GetFromJsonAsync<Question>($"/api/Question/GetBy/{id}");
                if (existing == null) return false;
                existing.Note = text;
                var update = await _httpClient.PutAsJsonAsync($"/api/Question/Pus/{id}", existing);
                if (!update.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Cập nhật thất bại: {(int)update.StatusCode} - {update.ReasonPhrase}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // Lịch sử của học sinh

        public async Task<List<lispackage>> GetAllHistoriesForStudent(string studentCode, int subjectId, DateTime start, DateTime end)
        {
            try
            {
                var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
                var student = students.FirstOrDefault(s => s.Student_Code == studentCode);
                if (student == null) return new List<lispackage>();

                var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
                var examRoomStudentList = examRoomStudents.Where(x => x.Student_Id == student.Id).ToList();
                if (!examRoomStudentList.Any()) return new List<lispackage>();

                var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var examRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
                var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
                var packageTypes = await _httpClient.GetFromJsonAsync<List<Package_Type>>("/api/Package_Type/Get");
                var examRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
                var reviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get") ?? new List<Review_Test>();

                var studentTests = tests.Where(t => examRoomStudentList.Any(ers => ers.Test_Id == t.Id)).ToList();
                var packageIds = studentTests.Select(x => x.Package_Id).Distinct().ToList();

                var packagesFiltered = packages
                    .Where(x => packageIds.Contains(x.Id) && x.Subject_Id == subjectId)
                    .ToList();

                var result = new List<lispackage>();

                foreach (var pack in packagesFiltered)
                {
                    // Lấy danh sách bài test của học sinh thuộc package này
                    var studentTestsInPackage = studentTests.Where(t => t.Package_Id == pack.Id).ToList();

                    // Đếm số lần review thành công (status == 2) của học sinh cho package này
                    int countReview = studentTestsInPackage.Count(test =>
                         reviewTests.Any(rt =>
                             rt.Test_Id == test.Id &&
                             rt.Student_Id == student.Id &&
                             (rt.Status == 2 || rt.Status == 3)
                         )
                     );


                    var type = packageTypes.FirstOrDefault(pt => pt.Id == pack.Package_Type_Id);

                    result.Add(new lispackage
                    {
                        Id = pack.Id,
                        countReview = countReview,
                        Name_package = pack.Package_Name,
                        Name_Package_Type = type?.Package_Type_Name
                    });
                }

                // Lọc theo thời gian exam room
                if (start != default && end != default)
                {
                    var examRoomIds = examRoomPackages
                        .Where(x => packagesFiltered.Any(p => p.Id == x.Package_Id))
                        .Select(x => x.Exam_Room_Id)
                        .Distinct()
                        .ToList();

                    var examRoomsFiltered = examRooms
                        .Where(er => examRoomIds.Contains(er.Id))
                        .Where(er =>
                        {
                            var roomStart = ConvertLong.ConvertLongToDateTime(er.Start_Time);
                            var roomEnd = ConvertLong.ConvertLongToDateTime(er.End_Time);
                            return roomEnd >= start && roomStart <= end;
                        }).ToList();

                    var validPackageIds = examRoomPackages
                        .Where(x => examRoomsFiltered.Any(er => er.Id == x.Exam_Room_Id))
                        .Select(x => x.Package_Id)
                        .Distinct()
                        .ToList();

                    result = result.Where(x => validPackageIds.Contains(x.Id)).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<listTest>> GetTestsForStudent(int packageId, string studentCode)
        {
            try
            {
                // Lấy danh sách học sinh và user
                var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new List<Student>();
                var users = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new List<User>();

                // Tìm student đúng code
                var student = students.FirstOrDefault(s =>
                    !string.IsNullOrWhiteSpace(s.Student_Code) &&
                    s.Student_Code.Trim().Equals(studentCode.Trim(), StringComparison.OrdinalIgnoreCase));
                if (student == null)
                {
                    Console.WriteLine($"Không tìm thấy student với Student_Code: [{studentCode}]");
                    return new List<listTest>();
                }

                // Lấy tên từ bảng User
                var user = users.FirstOrDefault(u => u.Id == student.User_Id);
                string studentName = user?.Full_Name ?? "N/A";

                // Lấy dữ liệu từ các bảng liên quan
                var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get") ?? new List<Data_Base.Models.T.Test>();
                var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new List<Exam_Room_Student>();
                var examHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get") ?? new List<Exam_HisTory>();
                var reviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get") ?? new List<Review_Test>();

                // Lọc ra các bài test thuộc gói thi
                var filteredTests = tests.Where(t => t.Package_Id == packageId).ToList();

                var result = new List<listTest>();

                foreach (var test in filteredTests)
                {
                    // Kiểm tra học sinh có trong danh sách phân công thi không
                    var examStudent = examRoomStudents.FirstOrDefault(e => e.Test_Id == test.Id && e.Student_Id == student.Id);
                    if (examStudent == null)
                        continue; // Bỏ qua nếu học sinh không được phân công thi bài này

                    // Tìm bản review test nếu có
                    //var reviewTest = reviewTests.FirstOrDefault(rt => rt.Student_Id == student.Id && rt.Test_Id == test.Id);
                    var reviewTest = await Task.Run(() => reviewTests?.FirstOrDefault(rt => rt.Test_Id == test.Id && rt.Student_Id == student?.Id));

                    double score = 0;
                    DateTime checkTime = DateTime.MinValue;
                    DateTime endTime = DateTime.MinValue;

                    if (examStudent.Check_Time != null)
                        checkTime = ConvertLong.ConvertLongToDateTime(examStudent.Check_Time);

                    var examHistory = examHistories.FirstOrDefault(e => e.Exam_Room_Student_Id == examStudent.Id);
                    if (examHistory != null)
                    {
                        score = examHistory.Score;
                        endTime = ConvertLong.ConvertLongToDateTime(examHistory.Create_Time);
                    }

                    result.Add(new listTest
                    {
                        Id = test.Id,
                        studnetid = student.Id,
                        IdReview = reviewTest?.Id ?? 0,
                        Idpackage = test.Package_Id,
                        Test_Code = test.Test_Code ?? "N/A",
                        Status = test.Status,
                        statustest = reviewTest?.Status ?? 0,
                        Name_Student = studentName,
                        score = score,
                        Check_Time = checkTime,
                        End_Time = endTime
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Review_Test> Getinforreview(int idstudent, int idtest)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Review_Test>>($"/api/Review_Tests/Get");
            return result.FirstOrDefault(x => x.Student_Id == idstudent && x.Test_Id == idtest);

        }
        public async Task<Student> GetStudentByUserIdAsync(int userId)
        {
            var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            return students.FirstOrDefault(s => s.User_Id == userId); // hoặc s.Id == userId, tuỳ DB
        }
        public async Task<bool> UpdateReviewforteacher(int id, int status, string reply, int userid, double totalScore)
        {
            try
            {
                var getallTeacher = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get");
                var teacher = getallTeacher.FirstOrDefault(t => t.User_Id == userid).Id;
                if (teacher == null) return false;
                var reviewTest = await _httpClient.GetFromJsonAsync<Review_Test>($"/api/Review_Tests/GetBy/{id}");
                if (reviewTest == null)
                {
                    Console.WriteLine("Không tìm thấy bản ghi cần cập nhật.");
                    return false;
                }
                reviewTest.Status = status;
                reviewTest.Reason_For_Refusal = reply ?? "   ";
                reviewTest.Teacher_Id = teacher;
                var response = await _httpClient.PutAsJsonAsync($"/api/Review_Tests/Pus/{id}", reviewTest);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Cập nhật thất bại: {response.StatusCode}");
                    return false;
                }

                var updated = await _httpClient.GetFromJsonAsync<Review_Test>($"/api/Review_Tests/GetBy/{id}");
                if (updated == null || updated.Test_Id == null)
                {
                    Console.WriteLine("Không lấy được Test_Id sau khi cập nhật.");
                    return false;
                }

                var testId = updated.Test_Id.Value;
                return await CreateDraft(testId, totalScore);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đánh giá: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> CreateDraft(int idtest, double totalScore)
        {
            bool resu = false;
            try
            {
                if (idtest == null || idtest < 0)
                {
                    Error = "Không xác định được bài thi, xác nhận phúc thảo thất bại";
                    return resu;
                }

                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                         { "Test_Id", idtest.ToString() },
                    },
                };

                var lstExamRoomStudent = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filter);

                if (!lstExamRoomStudent.IsSuccessStatusCode)
                {
                    Error = "Lỗi gọi api!";
                    return resu;
                }

                var examRoomStudent = (await lstExamRoomStudent.Content.ReadFromJsonAsync<List<Exam_Room_Student>>()).Where(o => o.Is_Check_Review == 0).SingleOrDefault();

                Exam_Room_Student ers = new Exam_Room_Student();
                ers.Student_Id = examRoomStudent.Student_Id;
                ers.Is_Check_Out = 0;
                ers.Exam_Room_Package_Id = examRoomStudent.Exam_Room_Package_Id;
                ers.Check_Time = examRoomStudent.Check_Time;
                ers.Is_Check_Review = 1;
                ers.Test_Id = examRoomStudent.Test_Id;

                var examRoomStudentNew = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/Post", ers);
                if (!examRoomStudentNew.IsSuccessStatusCode)
                {
                    Error = "lỗi gọi Api lưu bài thúc thảo thấy bại!";
                    return resu;
                }

                int ERA = (await examRoomStudentNew.Content.ReadFromJsonAsync<Exam_Room_Student>()).Id;

                var filterExamHist = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        {"Exam_Room_Student_Id", examRoomStudent.Id.ToString() }
                    },
                };

                var lstExamHist = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_HisTory/common/get", filter);

                if (!lstExamHist.IsSuccessStatusCode)
                {
                    Error = "Lỗi gọi api!!";
                    return resu;
                }

                var examHis = (await lstExamHist.Content.ReadFromJsonAsync<List<Exam_HisTory>>()).SingleOrDefault();

                Exam_HisTory eh = new Exam_HisTory();
                eh.Exam_Room_Student_Id = ERA;
                eh.Actual_Execution_Time = examHis.Actual_Execution_Time;
                eh.Score = totalScore;
                eh.Create_Time = examHis.Create_Time;

                var examHisNew = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_HisTory/Post", eh);
                if (!examHisNew.IsSuccessStatusCode)
                {
                    Error = "lỗi gọi Api lưu bài thúc thảo thấy bại!!";
                    return resu;
                }

                var filterERSAH = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        {"Exam_Room_Student_Id", examRoomStudent.Id.ToString() }
                    },
                };

                var lstERSAH = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/common/get", filterERSAH);

                if (!lstERSAH.IsSuccessStatusCode)
                {
                    Error = "Lỗi gọi api!!!";
                    return resu;
                }

                var ERSAH = await lstERSAH.Content.ReadFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>();


                List<Exam_Room_Student_Answer_HisTory> lstanHis = new List<Exam_Room_Student_Answer_HisTory>();
                foreach (var item in ERSAH)
                {
                    Exam_Room_Student_Answer_HisTory anHis = new Exam_Room_Student_Answer_HisTory();
                    anHis.Answer_Id = item.Answer_Id;
                    anHis.Exam_Room_Student_Id = ERA;

                    lstanHis.Add(anHis);
                }

                var lstanHisNew = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/PostList", lstanHis);
                if (!lstanHisNew.IsSuccessStatusCode)
                {
                    Error = "lỗi gọi Api lưu bài thúc thảo thấy bại!!!";
                    return resu;
                }

                resu = true;
                return resu;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return resu;
            }
        }
        public async Task<bool> UpdateReviewAsync(int id, int status)
        {
            try
            {
                // Lấy đối tượng cần cập nhật
                var reviewTest = await _httpClient.GetFromJsonAsync<Review_Test>($"/api/Review_Tests/GetBy/{id}");
                if (reviewTest == null)
                {
                    Console.WriteLine("Không tìm thấy bản ghi cần cập nhật.");
                    return false;
                }

                // Cập nhật trạng thái
                reviewTest.Status = status;

                // Gửi yêu cầu cập nhật
                var response = await _httpClient.PutAsJsonAsync($"/api/Review_Tests/Pus/{id}", reviewTest);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Cập nhật thất bại: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đánh giá: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Test_Review>> GetinforTeacher()
        {
            try
            {
                var getallreview = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
                var reviewTests = getallreview?
                    .Where(x => x.Status == 1)
                    .ToList();

                if (reviewTests == null || reviewTests.Count == 0)
                {
                    Console.WriteLine("Không có đánh giá nào cho học sinh này.");
                    return null;
                }

                var getalltest = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
                var getallcclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
                var getallsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");

                var data = new List<Test_Review>();

                foreach (var review in reviewTests)
                {
                    var test = getalltest?.FirstOrDefault(t => t.Id == review.Test_Id);
                    if (test == null) continue;

                    var package = getallpackage?.FirstOrDefault(p => p.Id == test.Package_Id);
                    if (package == null) continue;

                    var subject = getallsubject?.FirstOrDefault(s => s.Id == package.Subject_Id);
                    var classs = getallcclass?.FirstOrDefault(c => c.Id == package.Class_Id);
                    data.Add(new Test_Review
                    {
                        status = review.Status,
                        classname = classs?.Class_Name ?? "N/A",
                        Name_subject = subject?.Subject_Name ?? "N/A",
                        Test_code = test.Test_Code ?? "N/A"
                    });
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin đánh giá: {ex.Message}");
                return null;
            }
        }
        public async Task<List<Test_Review>> Getinfor(int id)
        {
            try
            {
                var getllstudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
                var getstudent = getllstudent.FirstOrDefault(x => x.User_Id == id);
                if (getstudent == null) return null;

                var getallreview = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
                var reviewTests = getallreview?
                    .Where(x => x.Student_Id == getstudent.Id && (x.Status == 2 || x.Status == 3))
                    .ToList();

                if (reviewTests == null || reviewTests.Count == 0)
                {
                    Console.WriteLine("Không có đánh giá nào cho học sinh này.");
                    return null;
                }

                var getalltest = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var getallpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get");
                var getallsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");

                var data = new List<Test_Review>();

                foreach (var review in reviewTests)
                {
                    var test = getalltest?.FirstOrDefault(t => t.Id == review.Test_Id);
                    if (test == null) continue;

                    var package = getallpackage?.FirstOrDefault(p => p.Id == test.Package_Id);
                    if (package == null) continue;

                    var subject = getallsubject?.FirstOrDefault(s => s.Id == package.Subject_Id);

                    data.Add(new Test_Review
                    {
                        status = review.Status,
                        Name_subject = subject?.Subject_Name ?? "N/A",
                        Test_code = test.Test_Code ?? "N/A"
                    });
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin đánh giá: {ex.Message}");
                return null;
            }
        }


        public async Task<Review_Test> SendsReview(int studentid, int testid, string Reviewcoment)
        {
            try
            {
                var reviewTest = new Review_Test
                {
                    Test_Id = testid,
                    Student_Id = studentid,
                    Reason_For_Sending = Reviewcoment,
                    Reason_For_Refusal = "   ",
                    Status = 1
                };
                var response = await _httpClient.PostAsJsonAsync("/api/Review_Tests/Post", reviewTest);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Review_Test>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi yêu cầu đánh giá: {ex.Message}");
                return null;

            }
        }

        public async Task<listTest> GetTestByCodeAsync(string testCode, string studentCode)
        {
            try
            {
                // Lấy tất cả bài test của học sinh (có thể tối ưu bằng API riêng nếu server hỗ trợ)
                var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
                var student = students.FirstOrDefault(s => s.Student_Code == studentCode);
                if (student == null) return null;

                var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get");
                var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
                var reviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get");
                var examHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");

                var test = tests.FirstOrDefault(t => t.Test_Code == testCode);
                if (test == null) return null;

                var examStudent = examRoomStudents.FirstOrDefault(e => e.Test_Id == test.Id && e.Student_Id == student.Id);
                if (examStudent == null) return null;

                var reviewTest = reviewTests.FirstOrDefault(rt => rt.Test_Id == test.Id && rt.Student_Id == student.Id);
                var examHistory = examHistories.FirstOrDefault(h => h.Exam_Room_Student_Id == examStudent.Id);

                return new listTest
                {
                    Id = test.Id,
                    studnetid = student.Id,
                    IdReview = reviewTest?.Id ?? 0,
                    Idpackage = test.Package_Id,
                    Test_Code = test.Test_Code,
                    Status = test.Status,
                    statustest = reviewTest?.Status ?? 0,
                    Name_Student = student.Student_Code,
                    score = examHistory?.Score ?? 0,
                    Check_Time = ConvertLong.ConvertLongToDateTime(examStudent.Check_Time),
                    End_Time = examHistory != null ? ConvertLong.ConvertLongToDateTime(examHistory.Create_Time) : DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateReviewedQuestionAns(ListQuesAnsReview ado)
        {
            try
            {
                if (ado == null || ado.Answers == null || !ado.Answers.Any())
                {
                    Console.WriteLine("Không có dữ liệu câu trả lời để cập nhật.");
                    return false;
                }

                // Bước 1: Cập nhật từng câu trả lời
                foreach (var answer in ado.Answers)
                {
                    var answerToUpdate = new Data_Base.Models.A.Answers
                    {
                        Id = answer.AnswerId,
                        Answers_Name = answer.AnswersName,
                        Right_Answer = answer.Right_Answer,
                        Question_Id = ado.QuestionId // Đảm bảo có QuestionId nếu cần
                    };

                    var answerResponse = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Answers/Pus/{answer.AnswerId}", answerToUpdate);

                    if (!answerResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Cập nhật câu trả lời thất bại cho AnswerId {answer.AnswerId}");
                        return false;
                    }
                }

                // Bước 2: Kiểm tra lại Test và Package tương ứng sau khi cập nhật đáp án
                var packageId = ado.PackageId;  // Đã có từ ado
                var packageResponse = await _httpClient.GetFromJsonAsync<Data_Base.Models.P.Package>($"https://localhost:7187/api/Package/GetBy/{packageId}");

                if (packageResponse == null)
                {
                    Console.WriteLine("Không tìm thấy thông tin về Package.");
                    return false;
                }

                int subjectId = packageResponse.Subject_Id;  // Lấy SubjectId từ Package
                int pointTypeId = packageResponse.Point_Type_Id;  // Lấy PointTypeId từ Package
                var testsInPackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>($"/api/Test/Get");
                var tests = testsInPackage.Where(t => t.Package_Id == packageId).ToList();

                if (!tests.Any())
                {
                    Console.WriteLine("Không tìm thấy bài Test nào thuộc Package.");
                    return false;
                }

                // Bước 3: Cập nhật điểm cho học sinh sau khi sửa đáp án
                foreach (var test in tests)
                {
                    // 1. Lấy học sinh của bài thi này
                    var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
                    var examRoomStudent = examRoomStudents.FirstOrDefault(e => e.Test_Id == test.Id);
                    if (examRoomStudent == null) continue;
                    int studentId = examRoomStudent.Student_Id;

                    // 2. Lấy toàn bộ lịch sử câu trả lời
                    var allHis = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get");
                    var studentHis = allHis.Where(h => h.Exam_Room_Student_Id == examRoomStudent.Id).ToList();

                    // 3. Lấy danh sách câu hỏi thuộc Test này
                    var filterTQ = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Test_Id", test.Id.ToString() }
                        }
                    };
                    var TQ = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test_Question/common/get", filterTQ);
                    var lstTQ = await TQ.Content.ReadFromJsonAsync<List<Test_Question>>();
                    int totalQuestions = lstTQ.Count;

                    int correctCount = 0;
                    foreach (var testQuestion in lstTQ)
                    {
                        int questionId = testQuestion.Question_Id;

                        // 4. Lấy đáp án đúng
                        var filterAns = new CommonFilterRequest
                        {
                            Filters = new Dictionary<string, string>
                            {
                                { "Question_Id", questionId.ToString() }
                            }
                        };
                        var ansResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filterAns);
                        var answers = await ansResponse.Content.ReadFromJsonAsync<List<Answers>>();
                        var correctAnswerIds = answers.Where(a => a.Right_Answer == 1).Select(a => a.Id).ToList();

                        // 5. Lấy các đáp án học sinh đã chọn cho câu hỏi hiện tại
                        var studentSelectedIds = studentHis
                            .Where(sa => answers.Any(a => a.Id == sa.Answer_Id && a.Question_Id == questionId))
                            .Select(sa => sa.Answer_Id)
                            .ToList();

                        // 6. So sánh
                        bool isCorrect =
                            studentSelectedIds.Count == correctAnswerIds.Count &&
                            !studentSelectedIds.Except(correctAnswerIds).Any();

                        if (isCorrect) correctCount++;
                    }

                    double pointsPerQuestion = totalQuestions == 0 ? 0 : 10.0 / totalQuestions;
                    double newPoints = pointsPerQuestion * correctCount;
                    doubleScore = newPoints;

                    // 7. Lọc Score cần update
                    var filterScore = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Student_Id", studentId.ToString() },
                            { "Test_Id", test.Id.ToString() }
                        }
                    };

                    var responseScore = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Score/common/get", filterScore);
                    var score = (await responseScore.Content.ReadFromJsonAsync<List<Score>>()).SingleOrDefault();
                    if (score == null)
                    {
                        Console.WriteLine("Không tìm thấy điểm cho học sinh và bài thi này.");
                        continue;
                    }

                    // 8. Update điểm cho Score
                    score.Point = newPoints;
                    var scoreUpdateResponse = await _httpClient.PutAsJsonAsync($"/api/Score/Pus/{score.Id}", score);
                    if (!scoreUpdateResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Cập nhật điểm thất bại cho Test {test.Id} của học sinh {studentId}");
                        continue;
                    }
                }


                Console.WriteLine("✅ Cập nhật đáp án và điểm số thành công.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật đáp án và điểm số: {ex.Message}");
                return false;
            }
        }

        public async Task<Data_Base.Models.P.Package?> GetPackageAsync(int packageId)
        {
            return await _httpClient.GetFromJsonAsync<Data_Base.Models.P.Package>($"/api/Package/GetBy/{packageId}");
        }

        public async Task<Class?> GetClassByIdAsync(int classId)
        {
            return await _httpClient.GetFromJsonAsync<Class>($"/api/Class/GetBy/{classId}");
        }

        public async Task<List<TeacherWithName>> GetListTeacherSubJect(int packageId)
        {
            try
            {
                var pkg = await GetPackageAsync(packageId);
                if (pkg == null) return new List<TeacherWithName>();

                int subjectId = pkg.Subject_Id;
                int classId = pkg.Class_Id;
                int GVRaDe = pkg.Teacher_Id;

                var clas = await _httpClient.GetFromJsonAsync<Class>($"/api/Class/GetBy/{classId}");
                int GVCNId = clas?.Teacher_Id ?? 0;

                var allTeachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get") ?? new List<Teacher>();

                var filtered = allTeachers
                    .Where(t => t.Subject_Id == subjectId)       // đúng môn của package
                    .Where(t => t.Id != GVCNId)              // không phải GVCN của lớp
                    .Where(t => t.Id != GVRaDe)              // không phải giáo viên ra đề
                    .ToList();

                // Map tên 1 lượt để tránh gọi API theo từng teacher
                var users = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new List<User>();
                return filtered.Select(t => new TeacherWithName
                {
                    Id = t.Id,
                    FullName = users.FirstOrDefault(u => u.Id == t.User_Id)?.Full_Name ?? $"Teacher #{t.Id}"
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> AssignReviewToTeacherAsync(int reviewId, int teacherId, string? note = null)
        {
            try
            {
                // Lấy record review hiện tại
                var review = await _httpClient.GetFromJsonAsync<Review_Test>($"/api/Review_Tests/GetBy/{reviewId}");
                if (review == null) return false;

                if (review.Teacher_Id != null && review.Teacher_Id > 0)
                    return false;

                review.Teacher_Id = teacherId;

                var resp = await _httpClient.PutAsJsonAsync($"/api/Review_Tests/Pus/{reviewId}", review);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Teacher?> GetTeacherByUserIdAsync(int userId)
        {
            var allTeachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get")
                             ?? new List<Teacher>();
            return allTeachers.FirstOrDefault(t => t.User_Id == userId);
        }

        public async Task<List<listTest>> GetReviewedTestsForStudentIdAndTestId(int studentId, int testId)
        {
            try
            {
                var ersAll = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new();
                var ersReviewed = ersAll
                    .Where(x => x.Student_Id == studentId && x.Test_Id == testId && x.Is_Check_Review == 1)
                    .ToList();
                if (!ersReviewed.Any()) return new();

                var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get") ?? new();
                var examHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get") ?? new();
                var users = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new();
                var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new();
                var scoreAll = await _httpClient.GetFromJsonAsync<List<Score>>("/api/Score/Get") ?? new();
                var reviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get") ?? new();

                var student = students.FirstOrDefault(s => s.Id == studentId);
                var studentName = users.FirstOrDefault(u => u.Id == student?.User_Id)?.Full_Name ?? "N/A";

                var result = new List<listTest>();

                foreach (var ersNew in ersReviewed)
                {
                    var test = tests.FirstOrDefault(t => t.Id == ersNew.Test_Id);
                    if (test == null) continue;

                    // ExamHistory MỚI (sau phúc khảo)
                    var newHis = examHistories
                        .Where(h => h.Exam_Room_Student_Id == ersNew.Id)
                        .OrderByDescending(h => h.Create_Time)
                        .FirstOrDefault();

                    var scoreRecord = scoreAll.FirstOrDefault(s => s.Student_Id == studentId && s.Test_Id == test.Id);
                    var review = reviewTests.FirstOrDefault(rt => rt.Student_Id == studentId && rt.Test_Id == test.Id);

                    // === TÌM Exam_Room_Student CŨ (trước phúc khảo) ===
                    var ersOld = ersAll
                        .Where(x => x.Student_Id == studentId && x.Test_Id == test.Id && x.Is_Check_Review == 0)
                        // ưu tiên bài gần nhất theo Check_Time (nếu null thì theo Id)
                        .OrderByDescending(x => x.Check_Time)
                        .ThenByDescending(x => x.Id)
                        .FirstOrDefault();

                    // Exam_HisTory CŨ
                    Exam_HisTory? oldHis = null;
                    if (ersOld != null)
                    {
                        oldHis = examHistories
                            .Where(h => h.Exam_Room_Student_Id == ersOld.Id)
                            .OrderByDescending(h => h.Create_Time)
                            .FirstOrDefault();
                    }

                    result.Add(new listTest
                    {
                        Id = test.Id,
                        IdReview = review?.Id ?? 0,
                        Idpackage = test.Package_Id,
                        Test_Code = test.Test_Code ?? "N/A",
                        Status = test.Status,
                        statustest = review?.Status ?? 0,
                        Name_Student = studentName,

                        // Điểm/giờ SAU phúc khảo
                        score = newHis?.Score ?? 0,
                        Point = scoreRecord?.Point ?? 0,
                        Check_Time = ConvertLong.ConvertLongToDateTime(ersNew.Check_Time),
                        End_Time = newHis != null ? ConvertLong.ConvertLongToDateTime(newHis.Create_Time) : DateTime.MinValue,
                        ExamRoomStudentId = ersNew.Id,

                        // ====== THÔNG TIN CŨ ======
                        OldScore = oldHis?.Score,
                        OldEnd_Time = oldHis != null ? ConvertLong.ConvertLongToDateTime(oldHis.Create_Time) : (DateTime?)null,
                        OldExamRoomStudentId = ersOld?.Id,
                        OldExamHistoryId = oldHis?.Id
                    });
                }

                return result.OrderByDescending(x => x.End_Time).ToList();
            }
            catch
            {
                return null;
            }
        }


        public async Task<List<listquestion>> GetQuestionsByExamRoomStudentId(int examRoomStudentId)
        {
            try
            {
                var ers = await _httpClient.GetFromJsonAsync<Exam_Room_Student>($"/api/Exam_Room_Student/GetBy/{examRoomStudentId}");
                if (ers == null) return new();

                var testId = ers.Test_Id;

                var testQuestions = await _httpClient.GetFromJsonAsync<List<Test_Question>>("/api/Test_Question/Get") ?? new();
                var questions = await _httpClient.GetFromJsonAsync<List<Question>>("/api/Question/Get") ?? new();
                var question_type = await _httpClient.GetFromJsonAsync<List<Question_Type>>("/api/Question_Type/Get") ?? new();
                var question_level = await _httpClient.GetFromJsonAsync<List<Question_Level>>("/api/Question_Level/Get") ?? new();
                var histories = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>("/api/Exam_Room_Student_Answer_HisTory/Get") ?? new();
                var answers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get") ?? new();
                var reviewTests = await _httpClient.GetFromJsonAsync<List<Review_Test>>("/api/Review_Tests/Get") ?? new();

                var tqs = testQuestions.Where(x => x.Test_Id == testId).ToList();
                var qs = questions.Where(q => tqs.Any(tq => tq.Question_Id == q.Id)).ToList();
                var histForThisErs = histories.Where(h => h.Exam_Room_Student_Id == examRoomStudentId).ToList();

                var review = reviewTests.FirstOrDefault(rt => rt.Test_Id == testId && rt.Student_Id == ers.Student_Id);
                var reviewStatus = review?.Status;
                var reviewTeacherId = review?.Teacher_Id;

                var result = new List<listquestion>();
                foreach (var q in qs)
                {
                    var type = question_type.FirstOrDefault(x => x.Id == q.Question_Type_Id);
                    var level = question_level.FirstOrDefault(x => x.Id == q.Question_Level_Id);

                    var ansForQ = answers.Where(a => a.Question_Id == q.Id).ToList();
                    var chosenForQ = histForThisErs.Where(h => ansForQ.Any(a => a.Id == h.Answer_Id)).ToList();

                    result.Add(new listquestion
                    {
                        Id = q.Id,
                        statuss = reviewStatus,
                        question_type = type?.Package_Type_Id ?? 0, // giữ nguyên mapping như code gốc
                        question_lever = level?.Question_Level_Name ?? "",
                        Questions = q,
                        Answers = ansForQ,
                        Exam_Room_Student_Answer_HisTories = chosenForQ,
                        Teacher_Id = reviewTeacherId
                    });
                }
                return result;
            }
            catch
            {
                return null;
            }
        }



        public class TeacherWithName
        {
            public int Id { get; set; }
            public string FullName { get; set; }
        }

        public class Test_Review
        {
            public int status { get; set; }
            public string classname { get; set; }
            public string Name_subject { get; set; }
            public string Test_code { get; set; }
        }
        public class StudentTestHistory
        {
            public int TestId { get; set; }
            public string TestCode { get; set; }
            public string PackageName { get; set; }
            public double Score { get; set; }
            public int Status { get; set; }
            public DateTime? CheckTime { get; set; }
            public DateTime? EndTime { get; set; }
        }

        public class lispackage
        {
            public int Id { get; set; }
            public int countReview { get; set; }
            public string Name_package { get; set; }
            public string Name_Package_Type { get; set; }
        }
        public class listTest
        {
            public int Id { get; set; }
            public int IdReview { get; set; }
            public string comenttest { get; set; }
            public string comenteacher { get; set; }
            public int studnetid { get; set; }
            public int Idpackage { get; set; }
            public string Test_Code { get; set; }
            public int? statustest { get; set; }
            public int Status { get; set; }
            public string Name_Student { get; set; }
            public double score { get; set; }
            public double Point { get; set; }
            public DateTime Check_Time { get; set; }
            public DateTime End_Time { get; set; }

            public int? Teacher_Id { get; set; }
            public int ExamRoomStudentId { get; set; }
            public double? OldScore { get; set; }
            public DateTime? OldEnd_Time { get; set; }
            public int? OldExamRoomStudentId { get; set; }
            public int? OldExamHistoryId { get; set; }

        }
        public class listquestion
        {
            public int Id { get; set; }
            public int? statuss { get; set; }
            public int question_type { get; set; }
            public string question_lever { get; set; }
            public Question Questions { get; set; }
            public List<Answers> Answers { get; set; }
            public List<Exam_Room_Student_Answer_HisTory> Exam_Room_Student_Answer_HisTories { get; set; }

            public int? Teacher_Id { get; set; } 
        }

        public class ListQuesAnsReview
        {
            public int QuestionId { get; set; }
            public int? QuestionTypeId { get; set; }
            public int PackageId { get; set; }
            public string QuestionName { get; set; }
            public double? MaximumScore { get; set; }
            public int Leva { get; set; }
            public List<AnswerReview>? Answers { get; set; }
        }
        public class AnswerReview
        {
            public int AnswerId { get; set; }
            public string AnswersName { get; set; }
            public int? Right_Answer { get; set; }
            public int? QuestionId { get; set; }
            public bool IsCorrect
            {
                get => Right_Answer == 1;
                set => Right_Answer = value ? 1 : 0;
            }
        }

    }
}
