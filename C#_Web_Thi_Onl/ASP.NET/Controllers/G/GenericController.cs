using Data_Base.App_DbContext;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace ASP.NET.Controllers.G
{
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly GenericRepository<T> _repository;

        public GenericController(GenericRepository<T> repository)
        {
            _repository = repository;
        }

        // 🔵 GetAll
        [HttpGet("Get/")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // 🔵 GetById
        [HttpGet("GetBy/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 🔵 Create
        [HttpPost("Post")]
        public async Task<IActionResult> Create([FromBody] T entity)
        {
            if (entity == null) return BadRequest();

            string entityName = typeof(T).Name;

            foreach (var prop in entity.GetType().GetProperties())
            {
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    prop.SetValue(entity, null);
                }
            }

            if (entityName == "Student")
            {
                var lastCode = await _repository.GetLastStudentCodeAsync();
                entity.GetType().GetProperty("Student_Code")?.SetValue(entity, GenerateStudentCode(lastCode));
            }
            else if (entityName == "Teacher")
            {
                var userIdProperty = entity.GetType().GetProperty("User_Id");
                if (userIdProperty == null) return BadRequest("Missing UserId property!");

                int userId = (int)userIdProperty.GetValue(entity);
                var user = await _repository.GetByIdAsync<User>(userId);
                if (user == null) return BadRequest("User not found!");

                long yearOfBirth = user.Data_Of_Birth / (long)Math.Pow(10, user.Data_Of_Birth.ToString().Length - 4);
                var lastCode = await _repository.GetLastTeacherCodeAsync(yearOfBirth);
                entity.GetType().GetProperty("Teacher_Code")?.SetValue(entity, GenerateTeacherCode(yearOfBirth, lastCode));
            }
            else if (entityName == "Class")
            {
                var gradeIdProperty = entity.GetType().GetProperty("Grade_Id");
                if (gradeIdProperty == null) return BadRequest("Missing Grade_Id property!");

                int gradeId = (int)gradeIdProperty.GetValue(entity);

                var grade = await _repository.GetByIdAsync<Grade>(gradeId);
                if (grade == null) return BadRequest("Grade not found!");

                string gradeName = grade.Grade_Name;

                var lastCode = await _repository.GetLastClassCodeAsync(gradeId);
                entity.GetType().GetProperty("Class_Code")?.SetValue(entity, GenerateClassCode(lastCode, gradeName));
            }
            else if (entityName == "Grade")
            {
                var lastCode = await _repository.GetLastGradeCodeAsync();
                entity.GetType().GetProperty("Grade_Code")?.SetValue(entity, GenerateGradeCode(lastCode));
            }
            else if (entityName == "Package")
            {
                entity.GetType().GetProperty("Package_Code")?.SetValue(entity, GenerateRandomPackageCode());
            }
            else if (entityName == "Subject")
            {
                var lastCode = await _repository.GetLastSubjectCodeAsync();
                entity.GetType().GetProperty("Subject_Code")?.SetValue(entity, GenerateSubjectCode(lastCode));
            }
            else if (entityName == "Test")
            {
                var packageIdProperty = entity.GetType().GetProperty("Package_Id");
                if (packageIdProperty == null) return BadRequest("Missing Package_Id property!");

                int packageId = (int)packageIdProperty.GetValue(entity);
                var package = await _repository.GetByIdAsync<Package>(packageId);
                if (package == null) return BadRequest("Package not found!");

                int pointTypeId = package.Point_Type_Id; // Lấy Point_Type_Id từ bảng Package

                string testCode = await GenerateTestCodeAsync(pointTypeId);
                entity.GetType().GetProperty("Test_Code")?.SetValue(entity, testCode);
            }
            else if (entityName == "Room")
            {
                string roomCode = await GenerateRoomCodeAsync();
                entity.GetType().GetProperty("Room_Code")?.SetValue(entity, roomCode);
            }
            else if (entityName == "Summary")
            {
                var startDateProperty = entity.GetType().GetProperty("Start_Time");
                var endDateProperty = entity.GetType().GetProperty("End_Time");
                var lastNumberProperty = entity.GetType().GetProperty("Last_Number");

                if (startDateProperty == null || endDateProperty == null || lastNumberProperty == null)
                {
                    return BadRequest("Missing required properties (Start_Date, End_Date, Last_Number)!");
                }

                DateTime startDate = (DateTime)startDateProperty.GetValue(entity);
                DateTime endDate = (DateTime)endDateProperty.GetValue(entity);
                int lastNumber = (int)lastNumberProperty.GetValue(entity);

                string summaryCode = GenerateSummaryCode(startDate, endDate, lastNumber);
                entity.GetType().GetProperty("Summary_Code")?.SetValue(entity, summaryCode);
            }
            var createdEntity = await _repository.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity }, createdEntity);
        }

        // 🔵 Update
        [HttpPut("Pus/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            if (entity == null) return BadRequest();
            var updated = await _repository.UpdateAsync(entity);
            if (!updated) return NotFound();
            return NoContent();
        }

        // 🔴 Delete
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // 🔥 Hàm tạo Student_Code
        private string GenerateStudentCode(string lastCode)
        {
            string year = DateTime.Now.ToString("yy");
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"STU{year}{lastNumber + 1:D9}";
        }

        // 🔥 Hàm tạo Teacher_Code
        private string GenerateTeacherCode(long yearOfBirth, string lastCode)
        {
            string year = (yearOfBirth % 100).ToString("D2");
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"TEA{year}{lastNumber + 1:D9}";
        }

        // 🔥 Hàm tạo Class
        private string GenerateClassCode(string lastCode, string gradeName)
        {
            string year = DateTime.Now.ToString("yyyy");
            int lastIndex = 0;

            if (!string.IsNullOrEmpty(lastCode) && lastCode.Length >= 10)
            {
                string lastIndexStr = lastCode.Substring(lastCode.Length - 3);
                if (int.TryParse(lastIndexStr, out int parsedIndex))
                {
                    lastIndex = parsedIndex;
                }
            }

            int newIndex = lastIndex + 1;
            return $"CLS{year}{gradeName}{newIndex:D3}";
        }

        // 🔥 Hàm tạo Grade
        private string GenerateGradeCode(string lastCode)
        {
            string year = DateTime.Now.ToString("yyyy");
            int lastIndex = lastCode != null ? int.Parse(lastCode.Substring(7)) : 0;
            return $"GRD{year}{(lastIndex + 1):D3}";
        }

        private int GenerateRandomPackageCode()
        {
            Random random = new Random();
            return random.Next(10000000, 99999999); // Tạo số nguyên ngẫu nhiên có 8 chữ số
        }

        private string GenerateSubjectCode(string lastCode)
        {
            string yearSuffix = DateTime.Now.ToString("yy");
            int lastIndex = lastCode != null ? int.Parse(lastCode.Substring(5)) : 0;
            return $"SUB{yearSuffix}{(lastIndex + 1):D3}";
        }

        private string GetTestType(int pointTypeId)
        {
            return pointTypeId switch
            {
                1 => "ATT",
                2 => "T15",
                3 => "T45",
                4 => "MID",
                5 => "FIN",
                _ => "UNK" // UNK: Unknown, nếu có lỗi
            };
        }

        private async Task<string> GenerateTestCodeAsync(int pointTypeId)
        {
            string testType = GetTestType(pointTypeId);
            string year = DateTime.Now.Year.ToString();

            int lastNumber = await _repository.GetLastTestNumberAsync(year, testType);
            int newNumber = lastNumber + 1;

            return $"T{testType}{year}{newNumber:D5}"; // Ví dụ: TATT2024001, TFIN2024002
        }
        private async Task<string> GenerateRoomCodeAsync()
        {
            int lastNumber = await _repository.GetLastRoomNumberAsync();
            int newNumber = lastNumber + 1;

            return $"R{newNumber:D3}";
        }

        private string GenerateSummaryCode(DateTime startDate, DateTime endDate, int lastNumber)
        {
            int year = DateTime.Now.Year;
            string startStr = startDate.ToString("MMdd"); // Chuyển ngày tháng thành MMDD
            string endStr = endDate.ToString("MMdd");     // Chuyển ngày tháng kết thúc thành MMDD
            return $"S{year}_{startStr}_{endStr}_{lastNumber}";
        }
    }
}
