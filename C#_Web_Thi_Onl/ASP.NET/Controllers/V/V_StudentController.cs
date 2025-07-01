using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.V_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.V
{
    [Route("api/[controller]")]
    [ApiController]
    public class V_StudentController : GenericController<V_Student>
    {
        public V_StudentController(GenericRepository<V_Student> repository) : base(repository)
        {
        }
    }
}
