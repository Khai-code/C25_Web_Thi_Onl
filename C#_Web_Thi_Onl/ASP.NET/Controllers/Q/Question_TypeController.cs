using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.Q
{
    [Route("api/[controller]")]
    [ApiController]
    public class Question_TypeController : GenericController<Question_Type>
    {
        public Question_TypeController(GenericRepository<Question_Type> repository) : base(repository)
        {
        }
    }
}
