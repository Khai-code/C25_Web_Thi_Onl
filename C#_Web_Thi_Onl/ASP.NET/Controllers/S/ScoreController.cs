using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.S
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : GenericController<Score>
    {
        public ScoreController(GenericRepository<Score> repository) : base(repository)
        {
        }
    }
}
