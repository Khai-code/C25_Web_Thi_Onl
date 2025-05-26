using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.Q;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.Q
{
    [Route("api/[controller]")]
    [ApiController]
    public class Question_LevelController : GenericController<Question_Level>
    {
        public Question_LevelController(GenericRepository<Question_Level> repository) : base(repository)
        {
        }
    }
}
