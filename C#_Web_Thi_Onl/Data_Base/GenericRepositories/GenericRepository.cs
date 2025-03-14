using Data_Base.App_DbContext;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.GenericRepositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly Db_Context _context;
        private readonly DbSet<T> _dbSet;

        // Thư viện dùng chung để gọi CRUD cgi tất cả các bảng ko dùng DTO
        public GenericRepository(Db_Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // 🔵 GetAll
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // 🔵 GetById
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // 🟠 Create
        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 🔵 Update
        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🔴 Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🏫 Lấy mã Student lớn nhất từ DB
        public async Task<string> GetLastStudentCodeAsync()
        {
            return await _context.Set<Student>()
                .OrderByDescending(s => s.Student_Code)
                .Select(s => s.Student_Code)
                .FirstOrDefaultAsync();
        }

        // 🏫 Lấy mã Teacher lớn nhất từ DB
        public async Task<string> GetLastTeacherCodeAsync(int yearOfBirth)
        {
            string prefix = $"TEA{yearOfBirth % 100:D2}";

            return await _context.Set<Teacher>()
                .Where(t => t.Teacher_Code.StartsWith(prefix))
                .OrderByDescending(t => t.Teacher_Code)
                .Select(t => t.Teacher_Code)
                .FirstOrDefaultAsync();
        }

    }
}
