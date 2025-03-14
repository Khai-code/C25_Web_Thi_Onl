using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.T
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : GenericController<Teacher>
    {
        public TeacherController(GenericRepository<Teacher> repository) : base(repository)
        {
        }
    }
}
