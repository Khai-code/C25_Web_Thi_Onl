using Data_Base.App_DbContext;
using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;

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

        [HttpPost("common/get")]
        public async Task<IActionResult> GetWithDynamicFilters([FromBody] CommonFilterRequest request)
        {
            string entityName = typeof(T).Name;
            switch (entityName)
            {
                case "Question":
                    return await HandleQuestionFilter(request.Filters);
                default:
                    return BadRequest($"Không hỗ trợ entity type: {request.Entity}");
            }
        }

        private string GetTableName(object entity)
        {
            if (entity == null) return null;
            string entityName = typeof(T).Name;
            var type = entity.GetType();
            Console.WriteLine("Đang lấy tên bảng cho type: " + type.FullName); // debug

            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.Name))
            {
                return tableAttribute.Name;
            }
            return type.Name;
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
            else if (entityName == "Package")
            {
                entity.GetType().GetProperty("Package_Code")?.SetValue(entity, GenerateRandomPackageCode());
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
            else if (entityName == "Student_Class")
            {
                var classIdProperty = entity.GetType().GetProperty("Class_Id");
                if (classIdProperty == null) return BadRequest("Missing Class_Id property!");

                int classId = (int)classIdProperty.GetValue(entity);

                // Gọi hàm GetStudentCountByClassIdAsync
                var studentCount = await _repository.GetStudentCountByClassIdAsync(classId);

                // Lấy thông tin lớp
                var classEntity = await _repository.GetByIdAsync<Class>(classId);
                if (classEntity != null)
                {
                    classEntity.Number = studentCount;
                    await _repository.UpdateClassAsync(classEntity);
                }
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

        private int GenerateRandomPackageCode()
        {
            Random random = new Random();
            return random.Next(10000000, 99999999); // Tạo số nguyên ngẫu nhiên có 8 chữ số
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

        private async Task<IActionResult> HandleQuestionFilter(Dictionary<string, string> filters)
        {
            int? packageId = filters.ContainsKey("Package_Id") ? int.Parse(filters["Package_Id"]) : null;
            int? subjectId = filters.ContainsKey("Subject_Id") ? int.Parse(filters["Subject_Id"]) : null;
            string? keyword = filters.ContainsKey("Keyword") ? filters["Keyword"] : null;

            var result = await _repository.GetWithFilterAsync<Question>(q =>
                (!packageId.HasValue || q.Package_Id == packageId) &&
                (string.IsNullOrEmpty(keyword) || q.Question_Name.Contains(keyword))
            );

            return Ok(result);
        }

    }
}
