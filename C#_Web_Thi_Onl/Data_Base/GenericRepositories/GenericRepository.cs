using Data_Base.App_DbContext;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.G;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Data_Base.DTO_Import_Excel.QuestionImportDto;

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
        public async Task<List<T>> CreateListAsync(List<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
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
        public async Task<string> GetLastTeacherCodeAsync(long yearOfBirth)
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

        public async Task<int> GetLastTestNumberAsync(string year, string testType)
        {
            var lastTest = await _context.Tests
                .Where(t => t.Test_Code.StartsWith($"T{testType}{year}"))
                .OrderByDescending(t => t.Test_Code)
                .FirstOrDefaultAsync();

            if (lastTest == null) return 0;

            string lastNumberStr = lastTest.Test_Code.Substring(6, 5); // Lấy 3 số cuối
            return int.TryParse(lastNumberStr, out int lastNumber) ? lastNumber : 0;
        }

        public async Task<int> GetStudentCountByClassIdAsync(int classId)
        {
            return await _context.Set<Student_Class>()
                .Where(sc => sc.Class_Id == classId)
                .CountAsync();
        }

        public async Task<bool> UpdateClassAsync(Class classEntity)
        {
            _context.Set<Class>().Update(classEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<T>> GetWithFilterAsync<T>(Expression<Func<T, bool>> filter) where T : class
        {
            return await _context.Set<T>().Where(filter).ToListAsync();
        }

        public async Task<bool> DeleteRangeAsync(List<int> ids)
        {
            var entities = await _context.Set<T>()
                                         .Where(e => ids.Contains(EF.Property<int>(e, "Id")))
                                         .ToListAsync();

            if (entities == null || entities.Count == 0)
                return false;

            _context.Set<T>().RemoveRange(entities);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
