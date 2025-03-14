using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.S
{
    [Route("api/[controller]")]
    [ApiController]
    public class Subject_GradeController : GenericController<Subject_Grade>
    {
        public Subject_GradeController(GenericRepository<Subject_Grade> repository) : base(repository)
        {
        }
    }
}
