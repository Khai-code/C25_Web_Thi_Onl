using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.V_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.V
{
    [Route("api/[controller]")]
    [ApiController]
    public class V_Student_Controller : GenericController<V_Student>
    {
        public V_Student_Controller(GenericRepository<V_Student> repository) : base(repository)
        {
        }
    }
}
