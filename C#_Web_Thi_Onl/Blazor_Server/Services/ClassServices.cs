using Data_Base.Models.C;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
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
        public async Task<bool> CreateClassAsync(Class newClass)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/Class/Post", newClass);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] CreateClassAsync failed: {response.StatusCode} - {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] CreateClassAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            try
            {
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
                if (imageFile != null && imageFile.Size > 0)
                {
                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}"
                                            .Replace(" ", "_")
                                            .ToLower();
                    string fileExtension = Path.GetExtension(imageFile.Name);
                    string uniqueFileName = $"{safeFileName}{fileExtension}";

                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
                    Directory.CreateDirectory(uploadsFolder);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
                    }

                    // Gán avatar path cho user trước khi post
                    user.Avatar = $"/image/avatars/{uniqueFileName}";
                }

                // 2. Tạo user
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

                // 3. Nếu là Student → tạo bảng Student
                if (user.Role_Id == 1)
                {
                    Student student = new Student();
                    student.User_Id = createdUser.Id;

                    var studentResponse = await _client.PostAsJsonAsync("api/Student/Post", student);

                    if (!studentResponse.IsSuccessStatusCode)
                        return null;

                    var createdStudent = await studentResponse.Content.ReadFromJsonAsync<Student>();
                    return createdStudent?.Id;
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

        // Hàm bỏ dấu tiếng Việt
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<bool> UpdateStudent(int id, User user, IBrowserFile? imageFile)
        {
            try
            {

                // Xử lý avatar nếu người dùng chọn ảnh mới
                if (imageFile != null && imageFile.Size > 0)
                {
                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}"
                                           .Replace(" ", "_")
                                           .ToLower();
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

                    // Cập nhật đường dẫn avatar
                    user.Avatar = $"/image/avatars/{uniqueFileName}";
                }

                // Nếu không có avatar, dùng mặc định
                if (string.IsNullOrWhiteSpace(user.Avatar))
                {
                    user.Avatar = "/image/avatars/default-avatar.png";
                }

                // Cập nhật thông tin người dùng
                var userResponse = await _client.PutAsJsonAsync($"api/User/Pus/{id}", user);

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

                    // 3. Xoá các bản ghi Student_Class liên quan
                    var studentClasses = await _client.GetFromJsonAsync<List<Student_Class>>("api/Student_Class/Get");
                    var relatedStudentClasses = studentClasses?
                        .Where(sc => sc.Student_Id == studentId)
                        .ToList();

                    foreach (var sc in relatedStudentClasses)
                    {
                        var deleteSCResponse = await _client.DeleteAsync($"api/Student_Class/Delete/{sc.Id}");
                        if (deleteSCResponse.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"[INFO] Đã xoá Student_Class Id = {sc.Id}");
                        }
                    }

                    // 4. Xoá Student
                    var deleteStudentResponse = await _client.DeleteAsync($"api/Student/Delete/{studentId}");
                }

                // 5. Cuối cùng xoá User
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
                return "/image/avatars/default-avatar.png";
            }

            return user.Avatar;
        }
    }
}
