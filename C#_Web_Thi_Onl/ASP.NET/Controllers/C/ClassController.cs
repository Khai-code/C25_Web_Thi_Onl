using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.C;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.C
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : GenericController<Class>
    {
        public ClassController(GenericRepository<Class> repository) : base(repository)
        {
        }
    }
}
