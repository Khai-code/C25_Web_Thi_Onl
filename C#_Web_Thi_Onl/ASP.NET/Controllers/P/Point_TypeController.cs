using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.P;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.P
{
    [Route("api/[controller]")]
    [ApiController]
    public class Point_TypeController : GenericController<Point_Type>
    {
        public Point_TypeController(GenericRepository<Point_Type> repository) : base(repository)
        {
        }
    }
}
