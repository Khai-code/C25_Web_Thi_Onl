﻿using Data_Base.GenericRepositories;
using Data_Base.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : GenericController<Room>
    {
        public RoomController(GenericRepository<Room> repository) : base(repository)
        {
        }
    }
}
