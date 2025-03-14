using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.T
{
    [Route("api/[controller]")]
    [ApiController]
    public class Teacher_ClassController : GenericController<Teacher_Class>
    {
        public Teacher_ClassController(GenericRepository<Teacher_Class> repository) : base(repository)
        {
        }
    }
}
