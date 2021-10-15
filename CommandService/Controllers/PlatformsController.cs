using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandService.Dtos;
using AutoMapper;
using CommandService.Data;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepo _repo;

        public PlatformsController(IMapper mapper, ICommandRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetAllPlatforms()
        {
            var platforms = await _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));        
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("-->Inbound Post # Command Service");
            return Ok("Inbound test of from Platforms Controller");
        }
    }
}