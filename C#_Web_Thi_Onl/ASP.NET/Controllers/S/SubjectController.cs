using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.S
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : GenericController<Subject>
    {
        public SubjectController(GenericRepository<Subject> repository) : base(repository)
        {
        }
    }
}
