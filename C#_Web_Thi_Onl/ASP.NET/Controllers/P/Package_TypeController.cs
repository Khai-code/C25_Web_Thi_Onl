using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.P;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.P
{
    [Route("api/[controller]")]
    [ApiController]
    public class Package_TypeController : GenericController<Package_Type>
    {
        public Package_TypeController(GenericRepository<Package_Type> repository) : base(repository)
        {
        }
    }
}
