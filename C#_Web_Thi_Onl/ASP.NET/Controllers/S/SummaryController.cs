using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.S
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : GenericController<Summary>
    {
        public SummaryController(GenericRepository<Summary> repository) : base(repository)
        {
        }
    }
}
