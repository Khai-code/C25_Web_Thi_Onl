﻿using Blazor_Server.Pages;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
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
                    Id = testId,
                    question_type = type.Package_Type_Id,
                    question_lever = lever.Question_Level_Name,
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
                .Where(s => pointypeList.Any(pt => pt.Id == s.Point_Type_Id)&& subjectList.Any(sj=>sj.Id==s.Subject_Id)&& examRoomStudentList.Any(exstd=>exstd.Student_Id==s.Student_Id))
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
        public async Task<bool> UpdateAllAnswerScores(Dictionary<int, string> answerScores)
        {
            var allAnswers = await _httpClient.GetFromJsonAsync<List<Answers>>("/api/Answers/Get");

            if (allAnswers == null || !allAnswers.Any())
            {
                Console.WriteLine("❌ Không lấy được danh sách câu trả lời từ API.");
                return false;
            }

            foreach (var entry in answerScores)
            {
                int answerId = entry.Key;
                string score = entry.Value;

                var answer = allAnswers.FirstOrDefault(a => a.Id == answerId);
                if (answer == null)
                {
                    Console.WriteLine($"⚠ Không tìm thấy câu trả lời với ID {answerId}");
                    continue;
                }

                answer.Points_Earned =Convert.ToDouble( score);

                var updateResponse = await _httpClient.PutAsJsonAsync($"/api/Answers/Pus/{answerId}", answer);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Cập nhật điểm thất bại cho Answer ID {answerId}: {(int)updateResponse.StatusCode} - {updateResponse.ReasonPhrase}");
                    return false;
                }
            }

            Console.WriteLine("✅ Đã cập nhật điểm thành phần cho tất cả câu trả lời.");
            return true;
        }

        public async Task<bool> updatecomemt(int id, string? text)
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


        // Lịch sử của học sinh

        public async Task<List<lispackage>> GetAllHistoriesForStudent(string studentCode, int subjectId, DateTime start, DateTime end)
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

            var studentTests = tests.Where(t => examRoomStudentList.Any(ers => ers.Test_Id == t.Id)).ToList();
            var packageIds = studentTests.Select(x => x.Package_Id).Distinct().ToList();

            var packagesFiltered = packages
                .Where(x => packageIds.Contains(x.Id) && x.Subject_Id == subjectId)
                .ToList();

            var result = new List<lispackage>();
            foreach (var pack in packagesFiltered)
            {
                var type = packageTypes.FirstOrDefault(pt => pt.Id == pack.Package_Type_Id);
                result.Add(new lispackage
                {
                    Id = pack.Id,
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

        public async Task<List<listTest>> GetTestsForStudent(int packageId, string studentCode)
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

            // Lấy dữ liệu khác
            var tests = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.T.Test>>("/api/Test/Get") ?? new List<Data_Base.Models.T.Test>();
            var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new List<Exam_Room_Student>();
            var examHistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get") ?? new List<Exam_HisTory>();

            var filteredTests = tests.Where(t => t.Package_Id == packageId).ToList();
            var result = new List<listTest>();

            foreach (var test in filteredTests)
            {
                var examStudent = examRoomStudents.FirstOrDefault(e => e.Test_Id == test.Id && e.Student_Id == student.Id);

                double score = 0;
                DateTime checkTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;

                if (examStudent != null)
                {
                    if (examStudent.Check_Time != null)
                        checkTime = ConvertLong.ConvertLongToDateTime(examStudent.Check_Time);

                    var examHistory = examHistories.FirstOrDefault(e => e.Exam_Room_Student_Id == examStudent.Id);
                    if (examHistory != null)
                    {
                        score = examHistory.Score;
                        endTime = ConvertLong.ConvertLongToDateTime(examHistory.Create_Time);
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



        public async Task<Student> GetStudentByUserIdAsync(int userId)
        {
            var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            return students.FirstOrDefault(s => s.User_Id == userId); // hoặc s.Id == userId, tuỳ DB
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
