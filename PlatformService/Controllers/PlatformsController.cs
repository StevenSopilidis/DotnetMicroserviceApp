using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private IMapper _mapper;
        private readonly ICommandDataClient __commandDataClient;
        private readonly IMessageBusClient _messageBusClient;
        public PlatformsController(IPlatformRepo repo, IMapper mapper, 
        ICommandDataClient commandDataClient,IMessageBusClient messageBusClient)
        {
            _repo = repo;
            _mapper = mapper;
            __commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
        {
            var platforms = await _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            var platform = await _repo.GetPlatformById(id);
            if(platform == null)
                return NotFound();
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto dto)
        {
            var platformModel = _mapper.Map<Platform>(dto);
            _repo.CreatePlatform(platformModel);
            await _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            //sending message synchronously
            try
            {
                await __commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Could not send synchronously: {ex.Message}");                
            }

            //sending message asynchronously
            try
            {
                _messageBusClient.PublishNewPlatform(_mapper.Map<PlatformPublishedDto>(platformReadDto));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Could not send message asynchronously: {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }
}