﻿using Data_Base.Models.C;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Blazor_Server.Services
{
    public class ClassServices
    {
        private readonly HttpClient _client;

        public ClassServices(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Class>> GetAllClass()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Class>>("/api/Class/Get");
                Console.WriteLine($"GetAllClass response: {response?.Count ?? 0} classes");
                return response ?? new List<Class>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllClass: {ex.Message}");
                return new List<Class>();
            }
        }

        public async Task<bool> CreateClassWithTeacherAsync(ClassWithTeacherModel model)
        {
            try
            {
                var newClass = new Class
                {
                    Class_Name = model.ClassName,
                    Class_Code = string.Empty,
                    Max_Student = model.MaxStudent,
                    Grade_Id = model.GradeId,
                    Teacher_Id = model.TeacherId,
                    Number = 0

                };

                var response = await _client.PostAsJsonAsync("api/Class/Post", newClass);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Tạo Class thất bại: {err}");
                    return false;
                }

                var createdClass = await response.Content.ReadFromJsonAsync<Class>();
                if (createdClass == null)
                {
                    Console.WriteLine("[ERROR] Không đọc được Class trả về.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] CreateClassWithTeacherAsync: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeleteClassAsync(int classId)
        {
            try
            {
                // B1. Lấy danh sách Student_Class của classId này
                var studentClasses = await _client.GetFromJsonAsync<List<Student_Class>>("api/Student_Class/Get");
                var studentsInClass = studentClasses?
                    .Where(sc => sc.Class_Id == classId)
                    .ToList();

                if (studentsInClass != null && studentsInClass.Count > 0)
                {
                    // B2. Lấy danh sách Student để lấy User_Id
                    var students = await _client.GetFromJsonAsync<List<Student>>("api/Student/Get");

                    foreach (var sc in studentsInClass)
                    {
                        var student = students?.FirstOrDefault(s => s.Id == sc.Student_Id);
                        if (student != null)
                        {
                            int userId = student.User_Id;
                            // B3. Gọi hàm xóa học sinh (đã tối ưu logic xóa hết điểm, xóa student_class, xóa user, xóa avatar)
                            await DeleteStudent(userId); // Nếu muốn chờ lần lượt
                        }
                    }
                }

                // B4. Sau khi đã xóa hết học sinh (và dữ liệu liên quan), mới xóa lớp
                var response = await _client.DeleteAsync($"api/Class/Delete/{classId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DeleteClassAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
                return response ?? new List<Subject>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllSubjects: {ex.Message}");
                return new List<Subject>();
            }
        }

        public async Task<List<Point_Type>> GetAllPointTypes()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get");
                return response ?? new List<Point_Type>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllPointTypes: {ex.Message}");
                return new List<Point_Type>();
            }
        }

        public async Task<List<Summary>> GetAllSummaries()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Summary>>("/api/Summary/Get");
                return response ?? new List<Summary>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllSummaries: {ex.Message}");
                return new List<Summary>();
            }
        }


        public async Task<List<Score>> GetAllScore()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Score>>("api/Score/Get");
                Console.WriteLine($"GetAllScore response: {response?.Count ?? 0} scores");
                return response ?? new List<Score>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllScore: {ex.Message}");
                return new List<Score>();
            }
        }

        public async Task<Dictionary<int, string>> GetSubjectDictionary()
        {
            var subjects = await GetAllSubjects();
            return subjects.ToDictionary(s => s.Id, s => s.Subject_Name);
        }

        public async Task<Dictionary<int, string>> GetPointTypeDictionary()
        {
            var types = await GetAllPointTypes();
            return types.ToDictionary(p => p.Id, p => p.Point_Type_Name);
        }


        public async Task<List<User>> GetStudentsAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<User>>("/api/User/Get");
                // Lọc ra những User có Role_Id là 1 (Giả sử Role_Id 1 là Student)
                var students = response?.Where(u => u.Role_Id == 1).ToList() ?? new List<User>();
                Console.WriteLine($"Filtered {students.Count} students");
                return students;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStudentsAsync: {ex.Message}");
                return new List<User>();
            }
        }
        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            try
            {
                var teachers = await _client.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get");
                return teachers ?? new List<Teacher>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetAllTeachersAsync: {ex.Message}");
                return new List<Teacher>();
            }
        }

        public async Task<int?> CreateStuent(User user, IBrowserFile? imageFile)
        {
            try
            {
                // 1. Tạo user trước
                var userResponse = await _client.PostAsJsonAsync("api/User/Post", user);
                if (!userResponse.IsSuccessStatusCode)
                {
                    var errorContent = await userResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Failed to create user: {userResponse.StatusCode}, {errorContent}");
                    return null;
                }

                var createdUser = await userResponse.Content.ReadFromJsonAsync<User>();
                if (createdUser == null)
                {
                    Console.WriteLine("[ERROR] Cannot deserialize created user.");
                    return null;
                }

                // 2. Nếu là Student → tạo bảng Student
                if (user.Role_Id == 1)
                {
                    var student = new Student
                    {
                        User_Id = createdUser.Id,
                        Student_Code = string.Empty // để backend tự sinh
                    };

                    var studentResponse = await _client.PostAsJsonAsync("api/Student/Post", student);
                    if (!studentResponse.IsSuccessStatusCode)
                        return null;

                    var createdStudent = await studentResponse.Content.ReadFromJsonAsync<Student>();
                    if (createdStudent == null)
                        return null;

                    // Lấy toàn bộ Point_Type để đối chiếu tên
                    var pointTypes = await _client.GetFromJsonAsync<List<Point_Type>>("api/Point_Type/Get");
                    var pointTypeSubjects = await _client.GetFromJsonAsync<List<Point_Type_Subject>>("api/Point_Type_Subject/Get");
                    var summaries = await GetAllSummaries();

                    foreach (var pts in pointTypeSubjects)
                    {
                        // Lấy tên của loại điểm
                        var pointType = pointTypes.FirstOrDefault(pt => pt.Id == pts.Point_Type_Id);
                        if (pointType == null) continue;

                        int quantity = 1;
                        switch (pointType.Point_Type_Name)
                        {
                            case "Attendance":
                            case "Point_15":
                                quantity = 3;
                                break;
                            case "Point_45":
                                quantity = 2;
                                break;
                            case "Point_Midterm":
                            case "Point_Final":
                                quantity = 1;
                                break;
                            default:
                                quantity = 1;
                                break;
                        }

                        foreach (var summary in summaries)
                        {
                            for (int i = 0; i < quantity; i++)
                            {
                                var score = new Score
                                {
                                    Student_Id = createdStudent.Id,
                                    Subject_Id = pts.Subject_Id,
                                    Point_Type_Id = pts.Point_Type_Id,
                                    Summary_Id = summary.Id,
                                    Test_Id = null,
                                    Point = 0,
                                };

                                var scoreResponse = await _client.PostAsJsonAsync("api/Score/Post", score);
                                if (!scoreResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"[WARNING] Không thể tạo điểm: Student {createdStudent.Id}, Subject {pts.Subject_Id}, PointType {pts.Point_Type_Id}, Summary {summary.Id}, Lần thứ {i + 1}");
                                }
                            }
                        }
                    }


                    // 3. Nếu có ảnh thì đặt tên theo Student_Code và lưu ảnh
                    if (imageFile != null && imageFile.Size > 0)
                    {
                        string safeFileName = createdStudent.Student_Code?.ToLower() ?? $"student_{createdStudent.Id}";
                        string fileExtension = Path.GetExtension(imageFile.Name);
                        string uniqueFileName = $"{safeFileName}{fileExtension}";

                        string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
                        Directory.CreateDirectory(uploadsFolder);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
                        }

                        // Cập nhật lại Avatar cho User
                        createdUser.Avatar = $"/image/avatars/{uniqueFileName}";
                        var updateAvatar = await _client.PutAsJsonAsync($"api/User/Pus/{createdUser.Id}", createdUser);
                        if (!updateAvatar.IsSuccessStatusCode)
                        {
                            Console.WriteLine("[WARNING] Không cập nhật được avatar.");
                        }
                    }

                    return createdStudent.Id;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] CreateStuent: {ex}");
                return null;
            }
        }


        public async Task<bool> AddStudentToClass(int studentId, int classId)
        {
            try
            {
                var students = await _client.GetFromJsonAsync<List<Student>>("api/Student/Get");
                var student = students?.FirstOrDefault(s => s.Id == studentId);

                if (student == null)
                {
                    Console.WriteLine($"Không tìm thấy Student.Id = {studentId}");
                    return false;
                }

                var existing = await _client.GetFromJsonAsync<List<Student_Class>>("api/Student_Class/Get");
                if (existing.Any(sc => sc.Student_Id == student.Id && sc.Class_Id == classId))
                {
                    Console.WriteLine($"Student đã có trong lớp.");
                    return true;
                }

                var studentClass = new Student_Class
                {
                    Student_Id = student.Id,
                    Class_Id = classId
                };

                var response = await _client.PostAsJsonAsync("api/Student_Class/Post", studentClass);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi AddStudentToClass: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateStudent(int id, User user, IBrowserFile? imageFile)
        {
            try
            {
                var students = await _client.GetFromJsonAsync<List<Student>>("api/Student/Get");
                var student = students?.FirstOrDefault(s => s.User_Id == id);

                if (student == null)
                {
                    Console.WriteLine($"[ERROR] Không tìm thấy Student với User_Id = {id}");
                    return false;
                }

                if (imageFile != null && imageFile.Size > 0)
                {
                    // Sử dụng Student_Code làm tên file
                    string safeFileName = student.Student_Code?.ToLower() ?? $"student_{student.Id}";
                    string fileExtension = Path.GetExtension(imageFile.Name);
                    string uniqueFileName = $"{safeFileName}{fileExtension}";
                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        using (var stream = imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    user.Avatar = $"/image/avatars/{uniqueFileName}";
                }

                var userResponse = await _client.PutAsJsonAsync($"api/User/Pus/{id}", user);
                if (!userResponse.IsSuccessStatusCode)
                {
                    var err = await userResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Cập nhật user thất bại: {err}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Lỗi khi cập nhật học sinh: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeleteStudent(int userId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Bắt đầu xoá học sinh với UserId = {userId}");

                // 1. Lấy thông tin user để xoá avatar nếu có
                var users = await _client.GetFromJsonAsync<List<User>>("api/User/Get");
                var userToDelete = users?.FirstOrDefault(u => u.Id == userId);

                if (userToDelete == null)
                {
                    Console.WriteLine($"[WARNING] Không tìm thấy user với Id = {userId}");
                    return false;
                }

                // Xoá avatar nếu có
                if (!string.IsNullOrEmpty(userToDelete.Avatar))
                {
                    try
                    {
                        string avatarPath = Path.Combine("wwwroot", userToDelete.Avatar.TrimStart('/'));
                        if (File.Exists(avatarPath))
                        {
                            File.Delete(avatarPath);
                            Console.WriteLine("[INFO] Đã xoá ảnh đại diện.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Lỗi khi xoá ảnh đại diện: {ex.Message}");
                    }
                }

                // 2. Lấy bảng Student để tìm theo User_Id
                var students = await _client.GetFromJsonAsync<List<Student>>("api/Student/Get");
                var student = students?.FirstOrDefault(s => s.User_Id == userId);

                if (student != null)
                {
                    int studentId = student.Id;

                    // 3. Xoá các bản ghi Score liên quan tới student này
                    var scores = await _client.GetFromJsonAsync<List<Score>>("api/Score/Get");
                    var relatedScores = scores?.Where(s => s.Student_Id == studentId).ToList();
                    if (relatedScores != null && relatedScores.Count > 0)
                    {
                        foreach (var score in relatedScores)
                        {
                            var deleteScoreResponse = await _client.DeleteAsync($"api/Score/Delete/{score.Id}");
                            if (deleteScoreResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[INFO] Đã xoá Score Id = {score.Id}");
                            }
                            else
                            {
                                var err = await deleteScoreResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"[WARNING] Không xoá được Score Id = {score.Id}: {err}");
                            }
                        }
                    }

                    // 4. Xoá các bản ghi Student_Class liên quan
                    var studentClasses = await _client.GetFromJsonAsync<List<Student_Class>>("api/Student_Class/Get");
                    var relatedStudentClasses = studentClasses?
                        .Where(sc => sc.Student_Id == studentId)
                        .ToList();

                    if (relatedStudentClasses != null && relatedStudentClasses.Count > 0)
                    {
                        foreach (var sc in relatedStudentClasses)
                        {
                            var deleteSCResponse = await _client.DeleteAsync($"api/Student_Class/Delete/{sc.Id}");
                            if (deleteSCResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[INFO] Đã xoá Student_Class Id = {sc.Id}");
                            }
                            else
                            {
                                var err = await deleteSCResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"[WARNING] Không xoá được Student_Class Id = {sc.Id}: {err}");
                            }
                        }
                    }

                    // 5. Xoá Student
                    var deleteStudentResponse = await _client.DeleteAsync($"api/Student/Delete/{studentId}");
                    if (!deleteStudentResponse.IsSuccessStatusCode)
                    {
                        var err = await deleteStudentResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[ERROR] Không xoá được Student: {err}");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("[INFO] Đã xoá Student.");
                    }
                }

                // 6. Cuối cùng xoá User
                var deleteUserResponse = await _client.DeleteAsync($"api/User/Delete/{userId}");
                if (!deleteUserResponse.IsSuccessStatusCode)
                {
                    var err = await deleteUserResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Không xoá được User: {err}");
                    return false;
                }

                Console.WriteLine("[SUCCESS] Xoá học sinh thành công.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Lỗi trong DeleteStudent: {ex.Message}");
                return false;
            }
        }


        // Phương thức lấy danh sách StudentClass
        public async Task<List<Student_Class>> GetAllStudentClass()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
                Console.WriteLine($"GetAllStudentClass response: {response?.Count ?? 0} student-class relationships");
                return response ?? new List<Student_Class>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllStudentClass: {ex.Message}");
                return new List<Student_Class>();
            }
        }

        public async Task<List<Grade>> GetAllGrades()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<List<Grade>>("/api/Grade/Get");
                Console.WriteLine($"GetAllGrades response: {response?.Count ?? 0} grades");
                return response ?? new List<Grade>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllGrades: {ex.Message}");
                return new List<Grade>();
            }
        }

        // Phương thức lấy đường dẫn đầy đủ của avatar dựa trên User
        public string GetUserAvatarPath(User user)
        {
            if (string.IsNullOrEmpty(user.Avatar))
            {
                // Trả về avatar mặc định nếu không có
                return "/images/default-avatar.png";
            }

            return user.Avatar;
        }

        public async Task<Dictionary<string, List<DetailedStudentScore>>> GetStudentDetailedScoresByTermSplit(int userId)
        {
            var allScores = await GetAllScore();
            var allStudents = await _client.GetFromJsonAsync<List<Student>>("api/Student/Get");
            var pointTypes = await GetAllPointTypes();
            var summaries = await GetAllSummaries();
            var subjects = await GetAllSubjects();

            var student = allStudents.FirstOrDefault(s => s.User_Id == userId);
            if (student == null) return new();

            var scores = allScores.Where(s => s.Student_Id == student.Id).ToList();
            var pointTypeDict = pointTypes.ToDictionary(p => p.Id, p => p.Point_Type_Name);
            var subjectDict = subjects.ToDictionary(s => s.Id, s => s.Subject_Name);
            var summaryDict = summaries.ToDictionary(s => s.Id, s => s.Summary_Name);

            // Tạo dictionary chứa điểm của từng kỳ
            var result = new Dictionary<string, List<DetailedStudentScore>>();

            foreach (var summary in summaries)
            {
                string termName = summary.Summary_Name;
                var termScores = scores
                    .Where(s => s.Summary_Id == summary.Id)
                    .Select(score => new DetailedStudentScore
                    {
                        SubjectName = subjectDict.GetValueOrDefault(score.Subject_Id, "???"),
                        PointTypeName = pointTypeDict.GetValueOrDefault(score.Point_Type_Id, "???"),
                        Point = score.Point,
                        Semester_Name = termName
                    })
                    .ToList();

                result[termName] = termScores;
            }

            return result;
        }


        public async Task<bool> CreateExcel(MultipartFormDataContent content, int classId)
        {
            try
            {
                var response = await _client.PostAsync($"https://localhost:7187/api/Excel_/import-student-excel?classId={classId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Thành công: " + msg);
                    return true;
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("❌ Thất bại: " + err);
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }


    public class ClassWithTeacherModel
    {
        public string ClassName { get; set; }
        public int MaxStudent { get; set; }
        public int GradeId { get; set; }
        public int TeacherId { get; set; }
        public int Number { get; set; }
    }

    public class DetailedStudentScore
    {
        public string SubjectName { get; set; }
        public string PointTypeName { get; set; }
        public double Point { get; set; }
        public string Semester_Name { get; set; }
    }


}