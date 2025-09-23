using Data_Base.App_DbContext;
using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using Data_Base.V_Model;
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
                case "Package":
                    return await HandlePackageFilter(request.Filters);
                case "Answers":
                    return await HandleAnswersFilter(request.Filters);
                case "Student":
                    return await HandleStudentFilter(request.Filters);
                case "Student_Class":
                    return await HandleStudentClassFilter(request.Filters);
                case "Exam_Room_Package":
                    return await HandleExamRoomPackageFilter(request.Filters);
                case "Exam_Room":
                    return await HandleExamRoomFilter(request.Filters);
                case "Test":
                    return await HandleTestFilter(request.Filters);
                case "V_Package":
                    return await HandleVPackageFilter(request.Filters);
                case "V_Student":
                    return await HandleVStudentFilter(request.Filters);
                case "Exam_Room_Student":
                    return await HandleExamRoomStudentFilter(request.Filters);
                case "Test_Question":
                    return await HandleTestQuestionFilter(request.Filters);
                case "V_Test":
                    return await HandleVTestFilter(request.Filters);
                case "Score":
                    return await HandleScoreFilter(request.Filters);
                case "Exam_HisTory":
                    return await HandleExamHisToryFilter(request.Filters);
                case "Exam_Room_Teacher":
                    return await HandleExamRoomTeacher(request.Filters);
                case "Teacher":
                    return await HandleTeacher(request.Filters);
                case "User":
                    return await HandleUser(request.Filters);
                case "Exam_Room_Student_Answer_HisTory":
                    return await HandleExamRoomStudentAnswerHisToryFilter(request.Filters);
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
                    classEntity.Number = studentCount + 1;
                    await _repository.UpdateClassAsync(classEntity);
                }
            }

            var createdEntity = await _repository.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity }, createdEntity);
        }
        [HttpPost("PostList")]
        public async Task<IActionResult> CreateList([FromBody] List<T> entities)
        {
            if (entities == null || !entities.Any()) return BadRequest();

            string entityName = typeof(T).Name;

            int counter = 0;

            foreach (var entity in entities)
            {
                // Xóa các collection navigation để tránh lỗi vòng lặp
                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        prop.SetValue(entity, null);
                    }
                }

                if (entityName == "Student")
                {
                    // Lấy mã cuối 1 lần và tăng dần để tránh trùng mã
                    var lastCode = counter == 0 ? await _repository.GetLastStudentCodeAsync() : null;
                    string newCode = GenerateStudentCode(lastCode, counter);
                    entity.GetType().GetProperty("Student_Code")?.SetValue(entity, newCode);
                    counter++;
                }
                else if (entityName == "Teacher")
                {
                    var userIdProperty = entity.GetType().GetProperty("User_Id");
                    if (userIdProperty == null) return BadRequest("Missing UserId property!");

                    int userId = (int)userIdProperty.GetValue(entity);
                    var user = await _repository.GetByIdAsync<User>(userId);
                    if (user == null) return BadRequest("User not found!");

                    long yearOfBirth = user.Data_Of_Birth / (long)Math.Pow(10, user.Data_Of_Birth.ToString().Length - 4);

                    var lastCode = counter == 0 ? await _repository.GetLastTeacherCodeAsync(yearOfBirth) : null;
                    string newCode = GenerateTeacherCode(yearOfBirth, lastCode, counter);
                    entity.GetType().GetProperty("Teacher_Code")?.SetValue(entity, newCode);
                    counter++;
                }
                else if (entityName == "Class")
                {
                    var gradeIdProperty = entity.GetType().GetProperty("Grade_Id");
                    if (gradeIdProperty == null) return BadRequest("Missing Grade_Id property!");

                    int gradeId = (int)gradeIdProperty.GetValue(entity);
                    var grade = await _repository.GetByIdAsync<Grade>(gradeId);
                    if (grade == null) return BadRequest("Grade not found!");

                    var lastCode = counter == 0 ? await _repository.GetLastClassCodeAsync(gradeId) : null;
                    string newCode = GenerateClassCode(lastCode, grade.Grade_Name, counter);
                    entity.GetType().GetProperty("Class_Code")?.SetValue(entity, newCode);
                    counter++;
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

                    int pointTypeId = package.Point_Type_Id;
                    string testCode = await GenerateTestCodeAsync(pointTypeId);
                    entity.GetType().GetProperty("Test_Code")?.SetValue(entity, testCode);
                }
                else if (entityName == "Student_Class")
                {
                    var classIdProperty = entity.GetType().GetProperty("Class_Id");
                    if (classIdProperty == null) return BadRequest("Missing Class_Id property!");

                    int classId = (int)classIdProperty.GetValue(entity);
                    var studentCount = await _repository.GetStudentCountByClassIdAsync(classId);
                    var classEntity = await _repository.GetByIdAsync<Class>(classId);
                    if (classEntity != null)
                    {
                        classEntity.Number = studentCount;
                        await _repository.UpdateClassAsync(classEntity);
                    }
                }
            }

            var createdEntities = await _repository.CreateListAsync(entities);
            return Ok(createdEntities);
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

        [HttpPut("PusList")]
        public async Task<IActionResult> UpdateList([FromBody] List<T> entities)
        {
            if (entities == null || !entities.Any())
                return BadRequest("Danh sách rỗng.");

            var updated = await _repository.UpdateListAsync(entities);
            if (!updated)
                return NotFound("Không có entity nào được cập nhật.");

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

        [HttpDelete("DeleteLst")]
        public async Task<IActionResult> DeleteRange([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Danh sách ID không được rỗng.");

            var deleted = await _repository.DeleteRangeAsync(ids);

            if (!deleted)
                return NotFound("Không tìm thấy một hoặc nhiều ID cần xóa.");

            return NoContent();
        }
        // 🔥 Hàm tạo Student_Code
        private string GenerateStudentCode(string lastCode, int offset = 0)
        {
            string year = DateTime.Now.ToString("yy");
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"STU{year}{lastNumber + 1 + offset:D9}";
        }
        // 🔥 Hàm tạo Teacher_Code
        private string GenerateTeacherCode(long yearOfBirth, string lastCode, int offset = 0)
        {
            string year = (yearOfBirth % 100).ToString("D2");
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"TEA{year}{lastNumber + 1 + offset:D9}";
        }
        // 🔥 Hàm tạo Class
        private string GenerateClassCode(string lastCode, string gradeName, int offset = 0)
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

            int newIndex = lastIndex + offset + 1;
            return $"CLS{year}{newIndex:D3}";
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

            return $"T{testType}{year}{newNumber:D5}";
        }
        private async Task<IActionResult> HandleQuestionFilter(Dictionary<string, string> filters)
        {
            List<int>? lstPackageId = filters.ContainsKey("Package_Id") && !string.IsNullOrEmpty(filters["Package_Id"])
                ? filters["Package_Id"].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x.Trim())).ToList() : new List<int>();
            //int? packageId = filters.ContainsKey("Package_Id") ? int.Parse(filters["Package_Id"]) : null;
            int? subjectId = filters.ContainsKey("Subject_Id") ? int.Parse(filters["Subject_Id"]) : null;
            List<int>? lstQuestionId = filters.ContainsKey("Id") && !string.IsNullOrEmpty(filters["Id"])
                ? filters["Id"].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x.Trim())).ToList() : new List<int>();
            string? keyword = filters.ContainsKey("Keyword") ? filters["Keyword"] : null;

            var result = await _repository.GetWithFilterAsync<Question>(q =>
                //(!packageId.HasValue || q.Package_Id == packageId) &&
                (!lstPackageId.Any() || lstPackageId.Contains(q.Package_Id)) &&
                (!lstQuestionId.Any() || lstQuestionId.Contains(q.Id)) &&
                (string.IsNullOrEmpty(keyword) || q.Question_Name.Contains(keyword))
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandlePackageFilter(Dictionary<string, string> filters)
        {
            int? packageCode = filters.ContainsKey("Package_Code") ? int.Parse(filters["Package_Code"]) : null;
            int? classId = filters.ContainsKey("Class_Id") ? int.Parse(filters["Class_Id"]) : null;
            int? packageTypeId = filters.ContainsKey("Package_Type_Id") ? int.Parse(filters["Package_Type_Id"]) : null;
            int? subjectId = filters.ContainsKey("Subject_Id") ? int.Parse(filters["Subject_Id"]) : null;
            string? keyword = filters.ContainsKey("Keyword") ? filters["Keyword"] : null;

            var result = await _repository.GetWithFilterAsync<Package>(p =>
                (!packageTypeId.HasValue || p.Package_Type_Id == packageTypeId) &&
                (!subjectId.HasValue || p.Subject_Id == subjectId) &&
                (!classId.HasValue || p.Class_Id == classId) &&
                (!packageCode.HasValue || p.Package_Code == packageCode) &&
                (string.IsNullOrEmpty(keyword) || p.Package_Name.Contains(keyword))
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleAnswersFilter(Dictionary<string, string> filters)
        {
            List<int>? lstquestionId = filters.ContainsKey("Question_Id") && !string.IsNullOrEmpty(filters["Question_Id"])
                ? filters["Question_Id"].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x.Trim())).ToList() : new List<int>();
            //int? questionId = filters.ContainsKey("Question_Id") ? int.Parse(filters["Question_Id"]) : null;
            string? keyword = filters.ContainsKey("Keyword") ? filters["Keyword"] : null;

            var result = await _repository.GetWithFilterAsync<Answers>(a =>
                (!lstquestionId.Any() || lstquestionId.Contains(a.Question_Id)) &&
                (string.IsNullOrEmpty(keyword) || a.Answers_Name.Contains(keyword))
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleStudentFilter(Dictionary<string, string> filters)
        {
            string? studentCode = filters.ContainsKey("Student_Code") ? filters["Student_Code"] : null;

            var result = await _repository.GetWithFilterAsync<Student>(a =>
                (!studentCode.Any() || a.Student_Code == studentCode)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleStudentClassFilter(Dictionary<string, string> filters)
        {
            int? studentId = filters.ContainsKey("Student_Id") ? int.Parse(filters["Student_Id"]) : null;
            string? keyword = filters.ContainsKey("Keyword") ? filters["Keyword"] : null;

            var result = await _repository.GetWithFilterAsync<Student_Class>(a =>
                (!studentId.HasValue || a.Student_Id == studentId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamRoomPackageFilter(Dictionary<string, string> filters)
        {
            int? packageId = filters.ContainsKey("Package_Id") ? int.Parse(filters["Package_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Exam_Room_Package>(a =>
                (!packageId.HasValue || a.Package_Id == packageId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamRoomFilter(Dictionary<string, string> filters)
        {
            int? id = filters.ContainsKey("Id") ? int.Parse(filters["Id"]) : null;
            int? roomId = filters.ContainsKey("Room_Id") ? int.Parse(filters["Room_Id"]) : null;
            List<int>? lstExamRoomid = filters.ContainsKey("Exam_Room_Id") && !string.IsNullOrEmpty(filters["Exam_Room_Id"])
                ? filters["Exam_Room_Id"].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x.Trim())).ToList() : new List<int>();

            var result = await _repository.GetWithFilterAsync<Exam_Room>(a =>
                (!id.HasValue || a.Id == id) &&
                (!roomId.HasValue || a.Room_Id == roomId) &&
                (!lstExamRoomid.Any() || lstExamRoomid.Contains(a.Id))
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleTestFilter(Dictionary<string, string> filters)
        {
            int? packageId = filters.ContainsKey("Package_Id") ? int.Parse(filters["Package_Id"]) : null;
            int? studentId = filters.ContainsKey("Student_Id") ? int.Parse(filters["Student_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.T.Test>(a =>
                (!packageId.HasValue || a.Package_Id == packageId) &&
                (!studentId.HasValue || a.Student_Id == studentId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleVPackageFilter(Dictionary<string, string> filters)
        {
            int? packageCode = filters.ContainsKey("Package_Code") ? int.Parse(filters["Package_Code"]) : null;
            int? packageId = filters.ContainsKey("Id") ? int.Parse(filters["Id"]) : null;
            int? classId = filters.ContainsKey("Class_Id") ? int.Parse(filters["Class_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<V_Package>(a =>
                (!packageCode.HasValue || a.Package_Code == packageCode) &&
                (!classId.HasValue || a.Class_Id == classId) &&
                (!packageId.HasValue || a.Id == packageId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleVStudentFilter(Dictionary<string, string> filters)
        {
            string? studentCode = filters.ContainsKey("Student_Code") ? filters["Student_Code"] : null;

            var result = await _repository.GetWithFilterAsync<V_Student>(a =>
                (!studentCode.Any() || a.Student_Code == studentCode)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleVTestFilter(Dictionary<string, string> filters)
        {
            int? Id = null;

            if (filters.TryGetValue("Id", out var idStr) && int.TryParse(idStr, out var parsedId))
            {
                Id = parsedId;
            }

            var result = await _repository.GetWithFilterAsync<V_Test>(a =>
                (!Id.HasValue || a.Id == Id)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamRoomStudentFilter(Dictionary<string, string> filters)
        {
            int? ExamRoomPackageId = filters.ContainsKey("Exam_Room_Package_Id") ? int.Parse(filters["Exam_Room_Package_Id"]) : null;
            int? studentId = filters.ContainsKey("Student_Id") ? int.Parse(filters["Student_Id"]) : null;
            int? testId = filters.ContainsKey("Test_Id") ? int.Parse(filters["Test_Id"]) : null;
            int? IsCheckOut = filters.ContainsKey("Is_Check_Out") ? int.Parse(filters["Is_Check_Out"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.E.Exam_Room_Student>(a =>
                (!ExamRoomPackageId.HasValue || a.Exam_Room_Package_Id == ExamRoomPackageId) &&
                (!studentId.HasValue || a.Student_Id == studentId) &&
                (!testId.HasValue || a.Test_Id == testId) &&
                (!IsCheckOut.HasValue || a.Is_Check_Out == IsCheckOut)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleTestQuestionFilter(Dictionary<string, string> filters)
        {
            int? testId = filters.ContainsKey("Test_Id") ? int.Parse(filters["Test_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.T.Test_Question>(a =>
                (!testId.HasValue || a.Test_Id == testId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleScoreFilter(Dictionary<string, string> filters)
        {
            int? studentId = filters.ContainsKey("Student_Id") ? int.Parse(filters["Student_Id"]) : null;
            int? subjectId = filters.ContainsKey("Subject_Id") ? int.Parse(filters["Subject_Id"]) : null;
            int? pointTypeId = filters.ContainsKey("Point_Type_Id") ? int.Parse(filters["Point_Type_Id"]) : null;
            int? summaryId = filters.ContainsKey("Summary_Id") ? int.Parse(filters["Summary_Id"]) : null;
            int? testId = filters.ContainsKey("Test_Id") ? int.Parse(filters["Test_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.S.Score>(a =>
                (!studentId.HasValue || a.Student_Id == studentId) &&
                (!subjectId.HasValue || a.Subject_Id == subjectId) &&
                (!pointTypeId.HasValue || a.Point_Type_Id == pointTypeId) &&
                (!summaryId.HasValue || a.Summary_Id == summaryId) &&
                (!testId.HasValue || a.Test_Id == testId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamHisToryFilter(Dictionary<string, string> filters)
        {
            int? Exam_Room_Student_Id = filters.ContainsKey("Exam_Room_Student_Id") ? int.Parse(filters["Exam_Room_Student_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.E.Exam_HisTory>(a =>
                (!Exam_Room_Student_Id.HasValue || a.Exam_Room_Student_Id == Exam_Room_Student_Id)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamRoomStudentAnswerHisToryFilter(Dictionary<string, string> filters)
        {
            int? Exam_Room_Student_Id = filters.ContainsKey("Exam_Room_Student_Id") ? int.Parse(filters["Exam_Room_Student_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory>(a =>
                (!Exam_Room_Student_Id.HasValue || a.Exam_Room_Student_Id == Exam_Room_Student_Id)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleExamRoomTeacher(Dictionary<string, string> filters)
        {
            int? examRoomId = filters.ContainsKey("Exam_Room_Id") ? int.Parse(filters["Exam_Room_Id"]) : null;
            int? TeacherId = filters.ContainsKey("Teacher_Id") ? int.Parse(filters["Teacher_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.E.Exam_Room_Teacher>(a =>
                (!examRoomId.HasValue || a.Exam_Room_Id == examRoomId) &&
                (!TeacherId.HasValue || a.Teacher_Id == TeacherId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleTeacher(Dictionary<string, string> filters)
        {
            int? userId = filters.ContainsKey("User_Id") ? int.Parse(filters["User_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.T.Teacher>(a =>
                (!userId.HasValue || a.User_Id == userId)
            );

            return Ok(result);
        }
        private async Task<IActionResult> HandleUser(Dictionary<string, string> filters)
        {
            int? Role_Id = filters.ContainsKey("Role_Id") ? int.Parse(filters["Role_Id"]) : null;

            var result = await _repository.GetWithFilterAsync<Data_Base.Models.U.User>(a =>
                (!Role_Id.HasValue || a.Role_Id == Role_Id)
            );

            return Ok(result);
        }
    }
}
