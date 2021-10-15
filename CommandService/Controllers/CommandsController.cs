using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
        {
            if(!(await _repo.PlatformExists(platformId)))
                return NotFound();
            var commands = await _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));   
        } 

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            if(!(await _repo.PlatformExists(platformId)))
                return NotFound();
            var command = await _repo.GetCommand(platformId, commandId);
            if(command == null)
                return NotFound();
            return Ok(_mapper.Map<CommandReadDto>(command));
        }   

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
        {
            if(!(await _repo.PlatformExists(platformId)))
                return NotFound();
            var command = _mapper.Map<Command>(commandCreateDto); 
            _repo.CreateCommand(platformId, command);
            await _repo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform), new {platformId=platformId}, commandReadDto); 
        }
    }
}