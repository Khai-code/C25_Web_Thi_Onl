using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.T
{
    [Route("api/[controller]")]
    [ApiController]
    public class Teacher_SubjectController : GenericController<Teacher_Subject>
    {
        public Teacher_SubjectController(GenericRepository<Teacher_Subject> repository) : base(repository)
        {
        }
    }
}
