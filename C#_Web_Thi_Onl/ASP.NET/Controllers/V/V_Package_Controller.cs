using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.U;
using Data_Base.V_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.V
{
    [Route("api/[controller]")]
    [ApiController]
    public class V_Package_Controller : GenericController<V_Package>
    {
        public V_Package_Controller(GenericRepository<V_Package> repository) : base(repository)
        {
        }
    }
}
