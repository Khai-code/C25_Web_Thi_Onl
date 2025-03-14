using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.A
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : GenericController<Answers>
    {
        public AnswersController(GenericRepository<Answers> repository) : base(repository)
        {
        }
    }
}
