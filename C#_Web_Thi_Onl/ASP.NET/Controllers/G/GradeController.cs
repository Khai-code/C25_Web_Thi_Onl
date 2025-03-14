using Data_Base.GenericRepositories;
using Data_Base.Models.G;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.G
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : GenericController<Grade>
    {
        public GradeController(GenericRepository<Grade> repository) : base(repository)
        {
        }
    }
}
