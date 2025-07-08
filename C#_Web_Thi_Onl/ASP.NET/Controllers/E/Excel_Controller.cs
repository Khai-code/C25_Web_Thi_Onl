using Data_Base.App_DbContext;
using Data_Base.Models.A;
using Data_Base.Models.Q;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data_Base.DTO_Import_Excel;
using OfficeOpenXml;
using System.Collections.Immutable;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Data_Base.Models.U;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class Excel_Controller : ControllerBase
    {
        private readonly Db_Context db_Context;
        private readonly HttpClient httpClient;
        private readonly IWebHostEnvironment _env;
        public Excel_Controller(Db_Context _Context,HttpClient _Client,IWebHostEnvironment webHostEnvironment)
        {
             db_Context = _Context;
            httpClient = _Client;
            _env = webHostEnvironment;
          
        }
        [HttpPost("import-student-excel")]
        public async Task<IActionResult> ImportStudentExcel(IFormFile file, [FromQuery] int classId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File rỗng");

            var studentsDto = new List<UserStudentImportDTO>();
            var relative = @"..\Blazor_Server\wwwroot";

            // Map sang đường dẫn tuyệt đối
            var blazorWwwroot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, relative));
            var saveDir = Path.Combine(blazorWwwroot, "images");
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            // 1. Đọc Excel và xử lý Avatar
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets[0];

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    object dobObj = sheet.Cells[row, 8].Value;
                    long dobLong = 0;
                    if (dobObj is DateTime dt)
                        dobLong = ConvertLong.ConvertDateTimeToLong(dt);
                    else if (dobObj != null && DateTime.TryParse(dobObj.ToString(), out var dt2))
                        dobLong = ConvertLong.ConvertDateTimeToLong(dt2);

                    long nowLong = ConvertLong.ConvertDateTimeToLong(DateTime.Now);

                    // Xử lý Avatar
                    string avatarPath = sheet.Cells[row, 9].Text?.Trim();
                    string avatarDbPath = null;
                    if (!string.IsNullOrEmpty(avatarPath) && System.IO.File.Exists(avatarPath))
                    {
                        try
                        {
                            string ext = Path.GetExtension(avatarPath);
                            string fileName = $"{Guid.NewGuid()}{ext}";
                            string destPath = Path.Combine(saveDir, fileName);
                            System.IO.File.Copy(avatarPath, destPath, true);
                            avatarDbPath = $"/images/{fileName}";
                        }
                        catch
                        {
                            avatarDbPath = null;
                        }
                    }

                    studentsDto.Add(new UserStudentImportDTO
                    {
                        Full_Name = sheet.Cells[row, 2].Text?.Trim(),
                        User_Name = sheet.Cells[row, 3].Text?.Trim(),
                        User_Pass = sheet.Cells[row, 4].Text?.Trim(),
                        Email = sheet.Cells[row, 5].Text?.Trim(),
                        Address = sheet.Cells[row, 6].Text?.Trim(),
                        Phone_Number = sheet.Cells[row, 7].Text?.Trim(),
                        Data_Of_Birth = dobLong,
                        Create_Time = nowLong,
                        Last_Mordification_Time = nowLong,
                        Avatar = avatarDbPath,
                        Status = 1,
                        Role_Id = 1
                    });
                }
            }

            // 2. Thêm Users vào DB, lấy đúng Id
            var addedUsers = new List<User>();
            foreach (var dto in studentsDto)
            {
                if (await db_Context.Users.AnyAsync(u => u.User_Name == dto.User_Name))
                    continue;

                var user = new User
                {
                    Full_Name = dto.Full_Name,
                    User_Name = dto.User_Name,
                    User_Pass = dto.User_Pass,
                    Email = dto.Email,
                    Address = dto.Address,
                    Phone_Number = dto.Phone_Number,
                    Data_Of_Birth = dto.Data_Of_Birth,
                    Create_Time = dto.Create_Time,
                    Last_Mordification_Time = dto.Last_Mordification_Time,
                    Avatar = dto.Avatar,
                    Status = dto.Status,
                    Role_Id = dto.Role_Id
                };
                db_Context.Users.Add(user);
                addedUsers.Add(user);
            }
            await db_Context.SaveChangesAsync(); // Phải save để lấy đúng Id

            // 3. Tạo list Student mapping đúng User_Id vừa tạo
            var students = new List<Student>();
            foreach (var user in addedUsers)
            {
                var student = new Student
                {
                    Student_Code = string.Empty,
                    User_Id = user.Id,
                };
                students.Add(student);
            }

            // 4. Gửi lên API PostList cho Student
            var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/Student/PostList", students);
            List<Student> createdStudents = null;
            if (response.IsSuccessStatusCode)
            {
                createdStudents = await response.Content.ReadFromJsonAsync<List<Student>>();
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                return BadRequest("Không import được student: " + err);
            }

            // 5. Tạo Student_Class với danh sách Student vừa nhận về (đã có Id)
            foreach (var student in createdStudents)
            {
                var studentClass = new Student_Class
                {
                    Student_Id = student.Id,
                    Class_Id = classId
                };
                db_Context.Student_Classes.Add(studentClass);
            }
            await db_Context.SaveChangesAsync();

            return Ok("Import học sinh thành công!");
        }

        //[HttpPost("exceltuluan")]
        //public async Task<IActionResult> ImportExcelTuLuan(IFormFile file, [FromQuery] int packageId)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File rỗng");

        //    // Lấy dữ liệu Type và Level từ DB
        //    var questionTypes = await db_Context.Question_Types.ToListAsync();
        //    var questionLevels = await db_Context.Question_Levels.ToListAsync();

        //    var questions = new List<QuestionImportEssay>();

        //    using var stream = file.OpenReadStream();
        //    using var package = new ExcelPackage(stream);
        //    var sheet = package.Workbook.Worksheets[0];

        //    for (int row = 4; row <= sheet.Dimension.End.Row; row++)
        //    {
        //        var qName = sheet.Cells[row, 2].Text?.Trim();
        //        var typeText = sheet.Cells[row, 3].Text?.Trim();
        //        var levelText = sheet.Cells[row, 4].Text?.Trim();
        //        var scoreText = sheet.Cells[row, 5].Text?.Trim();

        //        if (string.IsNullOrWhiteSpace(qName)) continue;

        //        // Tìm Id của Type theo tên
        //        var questionType = questionTypes
        //            .FirstOrDefault(x => x.Question_Type_Name.Trim().ToLower() == typeText?.ToLower());

        //        if (questionType == null)
        //            return BadRequest($"Không tìm thấy loại câu hỏi: {typeText}");

        //        // Tìm Id của Level theo tên
        //        var questionLevel = questionLevels
        //            .FirstOrDefault(x => x.Question_Level_Name.Trim().ToLower() == levelText?.ToLower());

        //        if (questionLevel == null)
        //            return BadRequest($"Không tìm thấy mức độ câu hỏi: {levelText}");

        //        long score = 0;
        //        if (!string.IsNullOrEmpty(scoreText))
        //            long.TryParse(scoreText, out score);

        //        questions.Add(new QuestionImportEssay
        //        {
        //            Question_Name = qName,
        //            Question_Type_Id = questionType.Id,
        //            Question_Level_Id = questionLevel.Id,
        //            Package_Id = packageId,
        //            Maximum_Score = score
        //        });
        //    }

        //    // Kiểm tra tổng điểm
        //    var totalScore = questions.Sum(x => x.Maximum_Score);
        //    if (totalScore != 10)
        //    {
        //        return BadRequest($"Tổng điểm các câu hỏi là {totalScore}. Tổng phải bằng đúng 10.");
        //    }

        //    // Lưu DB
        //    foreach (var dto in questions)
        //    {
        //        var question = new Question
        //        {
        //            Question_Name = dto.Question_Name,
        //            Question_Type_Id = dto.Question_Type_Id,
        //            Question_Level_Id = dto.Question_Level_Id,
        //            Package_Id = dto.Package_Id,
        //            Maximum_Score = dto.Maximum_Score
        //        };

        //        db_Context.Questions.Add(question);
        //    }

        //    await db_Context.SaveChangesAsync();

        //    return Ok("Thêm thành công!");
        //}
        [HttpPost("exceltuluan")]
        public async Task<IActionResult> ImportExcelTuLuan(IFormFile file, [FromQuery] int packageId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File rỗng");

            var typeMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var levelMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Bước 1: Đọc mapping từ Excel (DataSourceType & DataSourceLevel)
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                // Đọc Sheet DataSourceType
                var sheetType = package.Workbook.Worksheets["Danhmuc"];
                if (sheetType != null)
                {
                    for (int row = 1; row <= sheetType.Dimension.End.Row; row++)
                    {
                        var name = sheetType.Cells[row, 1].Text?.Trim();
                        var idText = sheetType.Cells[row, 2].Text?.Trim();

                        if (!string.IsNullOrEmpty(name) && int.TryParse(idText, out int id))
                        {
                            typeMapping[name] = id;
                        }
                    }
                }

                // Đọc Sheet DataSourceLevel
                var sheetLevel = package.Workbook.Worksheets["Danhmuc"];
                if (sheetLevel != null)
                {
                    for (int row = 1; row <= sheetLevel.Dimension.End.Row; row++)
                    {
                        var name = sheetLevel.Cells[row, 3].Text?.Trim();
                        var idText = sheetLevel.Cells[row, 4].Text?.Trim();

                        if (!string.IsNullOrEmpty(name) && int.TryParse(idText, out int id))
                        {
                            levelMapping[name] = id;
                        }
                    }
                }
            }

            // Bước 2: Đọc sheet dữ liệu chính (Import Data)
            var questions = new List<QuestionImportEssay>();

            using (var stream2 = file.OpenReadStream())
            using (var package2 = new ExcelPackage(stream2))
            {
                var sheet = package2.Workbook.Worksheets[0];

                for (int row = 4; row <= sheet.Dimension.End.Row; row++)
                {
                    var picture = sheet.Cells[row, 2].Text?.Trim();
                    var qName = sheet.Cells[row, 3].Text?.Trim();
                    var typeText = sheet.Cells[row, 4].Text?.Trim();
                    var levelText = sheet.Cells[row, 5].Text?.Trim();
                    var scoreText = sheet.Cells[row, 6].Text?.Trim();
                    byte[] pictureBytes = null;

                    if (!string.IsNullOrEmpty(picture) && System.IO.File.Exists(picture))
                    {
                        pictureBytes = System.IO.File.ReadAllBytes(picture);
                    }
                    if (string.IsNullOrWhiteSpace(qName)) continue;

                    if (!typeMapping.TryGetValue(typeText ?? "", out int questionTypeId))
                    {
                        return BadRequest($"Không tìm thấy loại câu hỏi: {typeText}");
                    }

                    if (!levelMapping.TryGetValue(levelText ?? "", out int questionLevelId))
                    {
                        return BadRequest($"Không tìm thấy mức độ câu hỏi: {levelText}");
                    }

                    double score = 0;
                    if (!string.IsNullOrEmpty(scoreText))
                        double.TryParse(scoreText, out score);

                    questions.Add(new QuestionImportEssay
                    {
                        Question_Name = qName,
                        Question_Type_Id = questionTypeId,
                        Question_Level_Id = questionLevelId,
                        Package_Id = packageId,
                        Maximum_Score = score,
                        pictures= pictureBytes
                    });
                }
            }

          

            foreach (var dto in questions)
            {
                var question = new Question
                {
                    Question_Name = dto.Question_Name,
                    Question_Type_Id = dto.Question_Type_Id,
                    Question_Level_Id = dto.Question_Level_Id,
                    Package_Id = dto.Package_Id,
                    Maximum_Score = dto.Maximum_Score,
                    Image=dto.pictures 
                };

                db_Context.Questions.Add(question);
            }

            await db_Context.SaveChangesAsync();

            return Ok("Thêm thành công!");
        }
        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file, [FromQuery] int packageId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File rỗng");
            var typeMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var levelMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Bước 1: Đọc mapping từ Excel (DataSourceType & DataSourceLevel)
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                // Đọc Sheet DataSourceType
                var sheetType = package.Workbook.Worksheets["DanhMuc"];
                if (sheetType != null)
                {
                    for (int row = 1; row <= sheetType.Dimension.End.Row; row++)
                    {
                        var name = sheetType.Cells[row, 1].Text?.Trim();
                        var idText = sheetType.Cells[row, 2].Text?.Trim();

                        if (!string.IsNullOrEmpty(name) && int.TryParse(idText, out int id))
                        {
                            typeMapping[name] = id;
                        }
                    }
                }

                // Đọc Sheet DataSourceLevel
                var sheetLevel = package.Workbook.Worksheets["DanhMuc"];
                if (sheetLevel != null)
                {
                    for (int row = 1; row <= sheetLevel.Dimension.End.Row; row++)
                    {
                        var name = sheetLevel.Cells[row, 3].Text?.Trim();
                        var idText = sheetLevel.Cells[row, 4].Text?.Trim();

                        if (!string.IsNullOrEmpty(name) && int.TryParse(idText, out int id))
                        {
                            levelMapping[name] = id;
                        }
                    }
                }
            }
            var questions = new List<QuestionImportDto>();

            using var stream2 = file.OpenReadStream();
            using var package2 = new ExcelPackage(stream2);
            var sheet = package2.Workbook.Worksheets[0];

            for (int row = 4; row <= sheet.Dimension.End.Row; row++)
            {
                var picture = sheet.Cells[row, 2].Text?.Trim();
                var qName = sheet.Cells[row, 3].Text?.Trim();
                var typeText = sheet.Cells[row, 4].Text.Trim();
                var levelText = sheet.Cells[row, 5].Text.Trim();
                byte[] pictureBytes = null;

                if (!string.IsNullOrEmpty(picture) && System.IO.File.Exists(picture))
                {
                    pictureBytes = System.IO.File.ReadAllBytes(picture);
                }
                if (!typeMapping.TryGetValue(typeText ?? "", out int questionTypeId))
                {
                    return BadRequest($"Không tìm thấy loại câu hỏi: {typeText}");
                }

                if (!levelMapping.TryGetValue(levelText ?? "", out int questionLevelId))
                {
                    return BadRequest($"Không tìm thấy mức độ câu hỏi: {levelText}");
                }

                var answers = new List<AnswerImportDto>();
                for (int col = 6; col <= sheet.Dimension.End.Column; col++)
                {
                    var cell = sheet.Cells[row, col].Text;
                    if (string.IsNullOrWhiteSpace(cell)) continue;

                    bool isCorrect = cell.Trim().StartsWith("*");
                    if (isCorrect) cell = cell.Trim().Substring(1).Trim();

                    answers.Add(new AnswerImportDto
                    {
                        Answers_Name = cell,
                        Right_Answer = isCorrect ? 1 : 0
                    });
                }

                questions.Add(new QuestionImportDto
                {
                    Question_Name = qName,
                    Question_Type_Id = questionTypeId,
                    Question_Level_Id = questionLevelId,
                    Package_Id = packageId,
                    picture = pictureBytes,
                    Answers = answers
                });
            }

            foreach (var dto in questions)
            {
                var question = new Question
                {
                    Question_Name = dto.Question_Name,
                    Question_Type_Id = dto.Question_Type_Id,
                    Question_Level_Id = dto.Question_Level_Id,
                    Package_Id = dto.Package_Id,
                    Image = dto.picture,
                };

                db_Context.Questions.Add(question);
                await db_Context.SaveChangesAsync();

                foreach (var ans in dto.Answers)
                {
                    db_Context.Answerses.Add(new Answers
                    {
                        Question_Id = question.Id,
                        Answers_Name = ans.Answers_Name,
                        Right_Answer = ans.Right_Answer
                    });
                }
            }

            await db_Context.SaveChangesAsync();
            return Ok("Thêm thành công!");
        }
    }
}
