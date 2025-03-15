using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.Q;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.Q
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : GenericController<Question>
    {
        public QuestionController(GenericRepository<Question> repository) : base(repository)
        {
        }
    }
}
