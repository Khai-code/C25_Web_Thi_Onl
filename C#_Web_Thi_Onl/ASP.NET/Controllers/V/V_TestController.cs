using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.V_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.V
{
    [Route("api/[controller]")]
    [ApiController]
    public class V_TestController : GenericController<V_Test>
    {
        public V_TestController(GenericRepository<V_Test> repository) : base(repository)
        {
        }
    }
}
