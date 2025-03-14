using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.L;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.L
{
    [Route("api/[controller]")]
    [ApiController]
    public class Learning_SummaryController : GenericController<Learning_Summary>
    {
        public Learning_SummaryController(GenericRepository<Learning_Summary> repository) : base(repository)
        {
        }
    }
}
