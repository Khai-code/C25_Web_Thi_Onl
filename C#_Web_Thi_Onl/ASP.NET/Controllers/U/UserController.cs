using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.U
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : GenericController<User>
    {
        public UserController(GenericRepository<User> repository) : base(repository)
        {
        }
    }
}
