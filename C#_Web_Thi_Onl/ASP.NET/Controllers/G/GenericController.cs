using Data_Base.App_DbContext;
using Data_Base.GenericRepositories;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASP.NET.Controllers.G
{
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly GenericRepository<T> _repository;
        private readonly Db_Context _context;

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
                var user = await _context.Set<User>().FindAsync(userId);
                if (user == null) return BadRequest("User not found!");

                int yearOfBirth = Convert.ToDateTime(user.Data_Of_Birth).Year;
                var lastCode = await _repository.GetLastTeacherCodeAsync(yearOfBirth);
                entity.GetType().GetProperty("Teacher_Code")?.SetValue(entity, GenerateTeacherCode(yearOfBirth, lastCode));
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
            string year = DateTime.Now.ToString("yy"); // Lấy 2 số cuối của năm hiện tại
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"STU{year}{lastNumber + 1:D9}";
        }

        // 🔥 Hàm tạo Teacher_Code
        private string GenerateTeacherCode(int yearOfBirth, string lastCode)
        {
            string year = (yearOfBirth % 100).ToString("D2"); // Lấy 2 số cuối của năm sinh
            long lastNumber = lastCode != null ? long.Parse(lastCode.Substring(5)) : 0;
            return $"TEA{year}{lastNumber + 1:D9}";
        }

    }
}
