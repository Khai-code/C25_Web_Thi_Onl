using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.S
{
    [Route("api/[controller]")]
    [ApiController]
    public class Student_ClassController : GenericController<Student_Class>
    {
        public Student_ClassController(GenericRepository<Student_Class> repository) : base(repository)
        {
        }
    }
}
