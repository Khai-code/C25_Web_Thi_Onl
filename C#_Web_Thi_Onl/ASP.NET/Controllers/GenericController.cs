using Data_Base.GenericRepositories;
using Data_Base.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        // 🔵 GetById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 🔵 Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T entity)
        {
            if (entity == null) return BadRequest();
            var createdEntity = await _repository.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity }, createdEntity);
        }

        // 🔵 Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            if (entity == null) return BadRequest();
            var updated = await _repository.UpdateAsync(entity);
            if (!updated) return NotFound();
            return NoContent();
        }

        // 🔴 Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
