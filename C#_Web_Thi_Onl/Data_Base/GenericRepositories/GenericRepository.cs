using Data_Base.App_DbContext;
using Data_Base.Models.C;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.GenericRepositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly Db_Context _context;

        // Thư viện dùng chung để gọi CRUD cgi tất cả các bảng ko dùng DTO
        public GenericRepository(Db_Context context)
        {
            _context = context;
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(object id) where TEntity : class
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        // 🔵 GetAll
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // 🔵 GetById
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // 🟠 Create
        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // 🔵 Update
        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // 🔴 Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
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

        public async Task<string> GetLastClassCodeAsync(int grade)
        {
            return await _context.Set<Class>()
            .Where(s => s.Grade_Id == grade) // Lọc theo Grade_Id thay vì Contains
            .OrderByDescending(s => s.Class_Code) // Sắp xếp giảm dần
            .Select(s => s.Class_Code)
            .FirstOrDefaultAsync();
        }

        public async Task<string> GetLastGradeCodeAsync()
        {
            return await _context.Set<Grade>()
            .Where(g => g.Grade_Code.StartsWith($"GRD{DateTime.Now:yyyy}"))
            .OrderByDescending(g => g.Grade_Code)
            .Select(g => g.Grade_Code)
            .FirstOrDefaultAsync();
        }

        public async Task<string> GetLastSubjectCodeAsync()
        {
            string yearSuffix = DateTime.Now.ToString("yy"); // Lấy 2 số cuối của năm
            return await _context.Set<Subject>()
                .Where(s => s.Subject_Code.StartsWith($"SUB{yearSuffix}"))
                .OrderByDescending(s => s.Subject_Code)
                .Select(s => s.Subject_Code)
                .FirstOrDefaultAsync();
        }
    }
}
