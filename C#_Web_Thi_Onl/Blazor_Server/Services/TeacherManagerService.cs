using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Text;
using static Blazor_Server.Services.PackageManager;

namespace Blazor_Server.Services
{
    public class TeacherManagerService
    {
        private readonly HttpClient _httpClient;
        public string ErrorMes { get; set; }
        public TeacherManagerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> CheckAccTeacher(listteacher user)
        {
            bool result = true;
            try
            {
                if (user == null)
                {
                    result = false;
                    ErrorMes = "Không có thông tin giáo viên";
                    return result;
                }

                var filterUser = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                         { "Role_Id", "2" },
                    },
                };

                var lstUserResul = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/User/common/get", filterUser);

                if (!lstUserResul.IsSuccessStatusCode)
                {
                    result = false;
                    ErrorMes = "Gọi api kiểm tra thông tin giáo viên thất bại";
                    return result;
                }

                List<User> lstUser = await lstUserResul.Content.ReadFromJsonAsync<List<Data_Base.Models.U.User>>();

                if (lstUser != null || lstUser.Count > 0)
                {
                    foreach (var item in lstUser)
                    {
                        if (item.User_Name == user.User_name)
                        {
                            result = false;
                            ErrorMes = "Tên tài khoản đã tồn tại";
                            return result;
                        }
                        else if (item.Email == user.Email)
                        {
                            result = false;
                            ErrorMes = "Email đã được sử dụng";
                            return result;
                        }
                        else if (item.Phone_Number == user.Phone_Number)
                        {
                            result = false;
                            ErrorMes = "Số điện thoại đã được sử dụng";
                            return result;
                        }
                        else if (item.Phone_Number == user.Phone_Number)
                        {
                            result = false;
                            ErrorMes = "Số điện thoại đã được sử dụng";
                            return result;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorMes = ex.Message;
                return false;
            }
        }
        public async Task<List<Subject>> GetAllSubject()
        {
            var listsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
            return listsubject;
        }
        public async Task<List<listteacher>> GetAllTeacher()
        {
            try
            {
                var ListUser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
                var ListTeacher = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get");
                var ListSubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");

                var teacherUsers = ListUser.Where(u => u.Role_Id == 2).ToList();
                List<listteacher> result = new List<listteacher>();

                foreach (var t in ListTeacher)
                {
                    var user = teacherUsers.FirstOrDefault(u => u.Id == t.User_Id);
                    if (user != null)
                    {
                        var subject = ListSubject.FirstOrDefault(s => s.Id == t.Subject_Id);

                        result.Add(new listteacher
                        {
                            Id = user.Id,
                            Full_Name = user.Full_Name,
                            User_name = user.User_Name,
                            PassWord = user.User_Pass,
                            Phone_Number = user.Phone_Number,
                            date_of_bith = user.Data_Of_Birth != null
                                ? ConvertLong.ConvertLongToDateTime(user.Data_Of_Birth)
                                : DateTime.MinValue,
                            Email = user.Email,
                            Address = user.Address,
                            Avatar = user.Avatar,
                            idsubject = subject?.Id ?? 0,
                            subject_name = subject != null ? subject.Subject_Name : "N/A",
                            Status = user.Status,
                            Position=t.Position
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] GetAllTeacher: {ex}");
                return new List<listteacher>();
            }
        }

        public async Task<int?> CreateTeacherAsync(listteacher user, IBrowserFile? imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Size > 0)
                {
                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}".Replace(" ", "_").ToLower();
                    string fileExtension = Path.GetExtension(imageFile.Name);
                    string uniqueFileName = $"{safeFileName}{fileExtension}";

                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
                    Directory.CreateDirectory(uploadsFolder);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
                    }

                    user.Avatar = $"/image/avatars/{uniqueFileName}";
                }

                var users = new User
                {
                    Full_Name = user.Full_Name,
                    Phone_Number = user.Phone_Number,
                    User_Name = user.User_name,
                    User_Pass = user.PassWord,
                    Email = user.Email,
                    Address = user.Address,
                    Data_Of_Birth = ConvertLong.ConvertDateTimeToLong(user.date_of_bith),
                    Create_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
                    Last_Mordification_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
                    Avatar = user.Avatar,
                    Status = user.Status,
                    Role_Id = int.Parse("2"),

                };
                var userResponse = await _httpClient.PostAsJsonAsync("api/User/Post", users);
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
                if (users.Role_Id == 2)
                {
                    var teacher = new Teacher
                    {
                        User_Id = createdUser.Id,
                        Teacher_Code = string.Empty,
                        Position= user.Position,
                        Subject_Id= user.idsubject
                    };
                    var teacherResponse = await _httpClient.PostAsJsonAsync("/api/Teacher/Post", teacher);

                    if (!teacherResponse.IsSuccessStatusCode)
                        return null;

                    var createdTeacher = await teacherResponse.Content.ReadFromJsonAsync<Teacher>();
                    if (createdTeacher == null) return null;

                    return createdTeacher.Id;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] CreateTeacherAsync: {ex}");
                return null;
            }
        }
        public async Task<bool> UpdateTeacherAsync(int id, listteacher user, IBrowserFile? imageFile)
        {
            try
            {
                var existingUser = await _httpClient.GetFromJsonAsync<User>($"/api/User/GetBy/{id}");
                if (existingUser == null)
                {
                    Console.WriteLine("[ERROR] Không tìm thấy user để lấy Create_Time.");
                    return false;
                }
                if (imageFile != null && imageFile.Size > 0)
                {
                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}".Replace(" ", "_").ToLower();
                    string fileExtension = Path.GetExtension(imageFile.Name);
                    string uniqueFileName = $"{safeFileName}{fileExtension}";

                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
                    Directory.CreateDirectory(uploadsFolder);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
                    }

                    user.Avatar = $"/image/avatars/{uniqueFileName}";
                }
                else
                {
                    user.Avatar = existingUser.Avatar;
                }

                var updatedUser = new User
                {
                    Id = user.Id,
                    Full_Name = user.Full_Name,
                    Phone_Number = user.Phone_Number,
                    User_Name = user.User_name,
                    User_Pass = user.PassWord,
                    Email = user.Email,
                    Address = user.Address,
                    Create_Time = existingUser.Create_Time,
                    Data_Of_Birth = ConvertLong.ConvertDateTimeToLong(user.date_of_bith),
                    Last_Mordification_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
                    Avatar = user.Avatar,
                    Status = user.Status,
                    Role_Id = existingUser.Role_Id,
                };

                var userResponse = await _httpClient.PutAsJsonAsync($"api/User/Pus/{id}", updatedUser);
                if (!userResponse.IsSuccessStatusCode)
                {
                    var errorContent = await userResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Cập nhật user thất bại: {userResponse.StatusCode}, {errorContent}");
                    return false;
                }
                var teachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("api/Teacher/Get");
                var teacher = teachers?.FirstOrDefault(t => t.User_Id == user.Id);
                if (teacher == null)
                {
                    Console.WriteLine("[ERROR] Không tìm thấy giáo viên.");
                    return false;
                }
                var teacherToUpdate = new Teacher
                {
                    Id=teacher.Id,
                    User_Id=teacher.User_Id,
                    Position = user.Position,
                    Teacher_Code=teacher.Teacher_Code,
                    Subject_Id = user.idsubject > 0 ? user.idsubject : null

                };
                var teacherResponse = await _httpClient.PutAsJsonAsync($"api/Teacher/Pus/{teacher.Id}", teacherToUpdate);
                if (!teacherResponse.IsSuccessStatusCode)
                {
                    var errorContent = await teacherResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Cập nhật giáo viên thất bại: {teacherResponse.StatusCode}, {errorContent}");
                    return false;
                }
                Console.WriteLine("[SUCCESS] Cập nhật giáo viên thành công.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] UpdateTeacherAsync: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteTeacher(int userId)
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<User>>("api/User/Get");
                var userToDelete = users?.FirstOrDefault(u => u.Id == userId);

                if (userToDelete == null)
                {
                    Console.WriteLine($"[WARNING] Không tìm thấy user với Id = {userId}");
                    return false;
                }
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
                var teachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("api/Teacher/Get");
                var teacher = teachers?.FirstOrDefault(t => t.User_Id == userId);

                if (teacher != null)
                {
                    var deleteTeacherResponse = await _httpClient.DeleteAsync($"api/Teacher/Delete/{teacher.Id}");
                }
                var deleteUserResponse = await _httpClient.DeleteAsync($"api/User/Delete/{userId}");
                if (!deleteUserResponse.IsSuccessStatusCode)
                {
                    var err = await deleteUserResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Không xoá được User: {err}");
                    return false;
                }

                Console.WriteLine("[SUCCESS] Xoá giáo viên thành công.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Lỗi trong DeleteTeacher: {ex.Message}");
                return false;
            }
        }

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
        public class listteacher
        {
            public int Id { get; set; }
            public string Full_Name { get; set; }
            public string Phone_Number { get; set; }
            public string User_name { get; set; }
            public string PassWord { get; set; }
            public string Email { get; set; }
            public DateTime date_of_bith { get; set; }
            public string Address { get; set; }
            public string Avatar { get; set; }
            public int Status { get; set; }
            public int Role_Id { get; set; }
            public int Position { get; set; }
            public int? idsubject { get; set; } // Mã môn dạy
            public string subject_name { get; set; } // Tên môn dạy
        }
    }
}
