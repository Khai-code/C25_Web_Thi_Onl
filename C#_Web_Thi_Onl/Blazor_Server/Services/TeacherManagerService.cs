//using Data_Base.GenericRepositories;
//using Data_Base.Models.S;
//using Data_Base.Models.T;
//using Data_Base.Models.U;
//using Microsoft.AspNetCore.Components.Forms;
//using System.Globalization;
//using System.Text;

//namespace Blazor_Server.Services
//{
//    public class TeacherManagerService
//    {
//        private readonly HttpClient _httpClient;
//        public TeacherManagerService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }
//        public async Task<List<Subject>> GetAllSubject()
//        {
//            var listsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get");
//            return listsubject;
//        }
//        public async Task<List<listteacher>> GetAllTeacher()
//        {
//            var ListUser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new List<User>();
//            var ListTeacher = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get") ?? new List<Teacher>();
//            var listsubject_teacher = await _httpClient.GetFromJsonAsync<List<Teacher_Subject>>("/api/Teacher_Subject/Get") ?? new List<Teacher_Subject>();
//            var listsubject = await _httpClient.GetFromJsonAsync<List<Subject>>("/api/Subject/Get") ?? new List<Subject>();
//            // Chỉ lấy user có Status = 1, 2 hoặc 3
//            var query = from user in ListUser
//                        join teacher in ListTeacher on user.Id equals teacher.User_Id
//                        where user.Status == 1 || user.Status == 2 || user.Status == 3
//                        select new { user, teacher };

//            var result = query.Select(t =>
//            {
//                var subjectNames = (from ts in listsubject_teacher
//                                    join subject in listsubject on ts.Subject_Id equals subject.Id
//                                    where ts.Teacher_id == t.teacher.Id
//                                    select subject.Subject_Name).ToList();

//                //return new listteacher
//                {
//                    Id = t.user.Id,
//                    Full_Name = t.user.Full_Name,
//                    User_name = t.user.User_Name,
//                    PassWord = t.user.User_Pass,
//                    Phone_Number = t.user.Phone_Number,
//                    date_of_bith = ConvertLong.ConvertLongToDateTime(t.user.Data_Of_Birth),
//                    Email = t.user.Email,
//                    Address = t.user.Address,
//                    Avatar = t.user.Avatar,
//                    Status = t.user.Status,
//                    Name_subject = string.Join(", ", subjectNames)
//                };
//            }).ToList();

//            return result;
//        }

//        public async Task<int?> CreateTeacherAsync(listteacher user, IBrowserFile? imageFile, int subjectIds)
//        {
//            try
//            {
//                if (imageFile != null && imageFile.Size > 0)
//                {
//                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}".Replace(" ", "_").ToLower();
//                    string fileExtension = Path.GetExtension(imageFile.Name);
//                    string uniqueFileName = $"{safeFileName}{fileExtension}";

//                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
//                    Directory.CreateDirectory(uploadsFolder);
//                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                    using (var fileStream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
//                    }

//                    user.Avatar = $"/image/avatars/{uniqueFileName}";
//                }

//                var users = new User
//                {
//                    Full_Name = user.Full_Name,
//                    Phone_Number = user.Phone_Number,
//                    User_Name = user.User_name,
//                    User_Pass = user.PassWord,
//                    Email = user.Email,
//                    Address = user.Address,
//                    Data_Of_Birth = ConvertLong.ConvertDateTimeToLong(user.date_of_bith),
//                    Create_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
//                    Last_Mordification_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
//                    Avatar = user.Avatar,
//                    Status = user.Status,
//                    Role_Id = int.Parse("2"),

//                };
//                var userResponse = await _httpClient.PostAsJsonAsync("api/User/Post", users);
//                if (!userResponse.IsSuccessStatusCode)
//                {
//                    var errorContent = await userResponse.Content.ReadAsStringAsync();
//                    Console.WriteLine($"[ERROR] Failed to create user: {userResponse.StatusCode}, {errorContent}");
//                    return null;
//                }

//                var createdUser = await userResponse.Content.ReadFromJsonAsync<User>();
//                if (createdUser == null)
//                {
//                    Console.WriteLine("[ERROR] Cannot deserialize created user.");
//                    return null;
//                }
//                if (users.Role_Id == 2)
//                {
//                    var teacher = new Teacher
//                    {
//                        User_Id = createdUser.Id,
//                        Teacher_Code = string.Empty
//                    };
//                    var teacherResponse = await _httpClient.PostAsJsonAsync("/api/Teacher/Post", teacher);

//                    if (!teacherResponse.IsSuccessStatusCode)
//                        return null;

//                    var createdTeacher = await teacherResponse.Content.ReadFromJsonAsync<Teacher>();
//                    if (createdTeacher == null) return null;
//                    var teacherSubject = new Teacher_Subject
//                    {
//                        Teacher_id = createdTeacher.Id,
//                        Subject_Id = subjectIds
//                    };

//                    var subjectResponse = await _httpClient.PostAsJsonAsync("/api/Teacher_Subject/Post", teacherSubject);
//                    if (!subjectResponse.IsSuccessStatusCode)
//                    {
//                        Console.WriteLine($"[ERROR] Failed to assign subject {subjectIds}");

//                        return null;
//                    }


//                    return createdTeacher.Id;
//                }

//                return null;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[EXCEPTION] CreateTeacherAsync: {ex}");
//                return null;
//            }
//        }
//        public async Task<bool> UpdateTeacherAsync(int id, listteacher user, IBrowserFile? imageFile, int subjectId)
//        {
//            try
//            {
//                var existingUser = await _httpClient.GetFromJsonAsync<User>($"/api/User/GetBy/{id}");
//                if (existingUser == null)
//                {
//                    Console.WriteLine("[ERROR] Không tìm thấy user để lấy Create_Time.");
//                    return false;
//                }
//                if (imageFile != null && imageFile.Size > 0)
//                {
//                    string safeFileName = $"{RemoveDiacritics(user.Full_Name)}".Replace(" ", "_").ToLower();
//                    string fileExtension = Path.GetExtension(imageFile.Name);
//                    string uniqueFileName = $"{safeFileName}{fileExtension}";

//                    string uploadsFolder = Path.Combine("wwwroot", "image", "avatars");
//                    Directory.CreateDirectory(uploadsFolder);
//                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                    using (var fileStream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await imageFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(fileStream);
//                    }

//                    user.Avatar = $"/image/avatars/{uniqueFileName}";
//                }
//                else
//                {
//                    user.Avatar = existingUser.Avatar;
//                }

//                var updatedUser = new User
//                {
//                    Id = user.Id,
//                    Full_Name = user.Full_Name,
//                    Phone_Number = user.Phone_Number,
//                    User_Name = user.User_name,
//                    User_Pass = user.PassWord,
//                    Email = user.Email,
//                    Address = user.Address,
//                    Create_Time = existingUser.Create_Time,
//                    Data_Of_Birth = ConvertLong.ConvertDateTimeToLong(user.date_of_bith),
//                    Last_Mordification_Time = ConvertLong.ConvertDateTimeToLong(DateTime.Now),
//                    Avatar = user.Avatar,
//                    Status = user.Status,
//                    Role_Id = existingUser.Role_Id,
//                };

//                var userResponse = await _httpClient.PutAsJsonAsync($"api/User/Pus/{id}", updatedUser);
//                if (!userResponse.IsSuccessStatusCode)
//                {
//                    var errorContent = await userResponse.Content.ReadAsStringAsync();
//                    Console.WriteLine($"[ERROR] Cập nhật user thất bại: {userResponse.StatusCode}, {errorContent}");
//                    return false;
//                }
//                var teachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("api/Teacher/Get");
//                var teacher = teachers?.FirstOrDefault(t => t.User_Id == user.Id);
//                if (teacher == null)
//                {
//                    Console.WriteLine("[ERROR] Không tìm thấy giáo viên.");
//                    return false;
//                }
//                var teacherSubjects = await _httpClient.GetFromJsonAsync<List<Teacher_Subject>>("api/Teacher_Subject/Get");
//                var oldSubjects = teacherSubjects?
//                    .Where(ts => ts.Teacher_id == teacher.Id)
//                    .ToList();

//                foreach (var ts in oldSubjects!)
//                {
//                    await _httpClient.DeleteAsync($"api/Teacher_Subject/Delete/{ts.Id}");
//                }

//                var newSubject = new Teacher_Subject
//                {
//                    Teacher_id = teacher.Id,
//                    Subject_Id = subjectId
//                };

//                var subjectResponse = await _httpClient.PostAsJsonAsync("api/Teacher_Subject/Post", newSubject);
//                if (!subjectResponse.IsSuccessStatusCode)
//                {
//                    Console.WriteLine($"[ERROR] Thêm môn dạy mới thất bại.");
//                    return false;
//                }

//                Console.WriteLine("[SUCCESS] Cập nhật giáo viên thành công.");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[EXCEPTION] UpdateTeacherAsync: {ex.Message}");
//                return false;
//            }
//        }
//        public async Task<bool> DeleteTeacher(int userId)
//        {
//            try
//            {
//                var users = await _httpClient.GetFromJsonAsync<List<User>>("api/User/Get");
//                var userToDelete = users?.FirstOrDefault(u => u.Id == userId);

//                if (userToDelete == null)
//                {
//                    Console.WriteLine($"[WARNING] Không tìm thấy user với Id = {userId}");
//                    return false;
//                }
//                if (!string.IsNullOrEmpty(userToDelete.Avatar))
//                {
//                    try
//                    {
//                        string avatarPath = Path.Combine("wwwroot", userToDelete.Avatar.TrimStart('/'));
//                        if (File.Exists(avatarPath))
//                        {
//                            File.Delete(avatarPath);
//                            Console.WriteLine("[INFO] Đã xoá ảnh đại diện.");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"[WARNING] Lỗi khi xoá ảnh đại diện: {ex.Message}");
//                    }
//                }
//                var teachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("api/Teacher/Get");
//                var teacher = teachers?.FirstOrDefault(t => t.User_Id == userId);

//                if (teacher != null)
//                {
//                    int teacherId = teacher.Id;
//                    var teacherSubjects = await _httpClient.GetFromJsonAsync<List<Teacher_Subject>>("api/Teacher_Subject/Get");
//                    var relatedTeacherSubjects = teacherSubjects?
//                        .Where(ts => ts.Teacher_id == teacherId)
//                        .ToList();

//                    foreach (var ts in relatedTeacherSubjects)
//                    {
//                        var deleteTSResponse = await _httpClient.DeleteAsync($"api/Teacher_Subject/Delete/{ts.Id}");
//                        if (deleteTSResponse.IsSuccessStatusCode)
//                        {
//                            Console.WriteLine($"[INFO] Đã xoá Teacher_Subject Id = {ts.Id}");
//                        }
//                    }
//                    var deleteTeacherResponse = await _httpClient.DeleteAsync($"api/Teacher/Delete/{teacherId}");
//                }
//                var deleteUserResponse = await _httpClient.DeleteAsync($"api/User/Delete/{userId}");
//                if (!deleteUserResponse.IsSuccessStatusCode)
//                {
//                    var err = await deleteUserResponse.Content.ReadAsStringAsync();
//                    Console.WriteLine($"[ERROR] Không xoá được User: {err}");
//                    return false;
//                }

//                Console.WriteLine("[SUCCESS] Xoá giáo viên thành công.");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[EXCEPTION] Lỗi trong DeleteTeacher: {ex.Message}");
//                return false;
//            }
//        }

//        private string RemoveDiacritics(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return text;

//            string normalized = text.Normalize(NormalizationForm.FormD);
//            var sb = new StringBuilder();

//            foreach (char c in normalized)
//            {
//                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
//                    sb.Append(c);
//            }

//            return sb.ToString().Normalize(NormalizationForm.FormC);
//        }
//        public class listteacher
//        {
//            public int Id { get; set; }
//            public string Full_Name { get; set; }
//            public string Phone_Number { get; set; }
//            public string User_name { get; set; }
//            public string PassWord { get; set; }
//            public string Email { get; set; }
//            public DateTime date_of_bith { get; set; }
//            public string Address { get; set; }
//            public string Avatar { get; set; }
//            public int Status { get; set; }
//            public int Role_Id { get; set; }
//            public string Name_subject { get; set; }
//            public int Idsubject { get; set; }
//        }
//    }
//}
