using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        //because EventProcessor is injected as signleton
        ///services with lower life time (scoped, transient)
        //cannot be injected via normall way
        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);     
            switch (eventType)
            {
                case EventTypes.PlatformPublished:
                    await AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventTypes DetermineEvent(string notificationEvent)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationEvent);
            switch (eventType.Event)
            {   
                case "Event_Published":
                    Console.WriteLine("Platform Published event detected");
                    return EventTypes.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine event type");
                    return EventTypes.Undetermined;
            }
        }
        private async Task AddPlatform(string platformPublishedMessage)
        {
            using(var scoped = _scopeFactory.CreateScope())
            {
                var repo = scoped.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if(( await repo.ExternalPlatformExists(platform.ExternalID) )== false)
                    {
                        repo.CreatePlatform(platform);
                        await repo.SaveChanges();
                    }
                     
                }
                catch (System.Exception)
                {
                    
                    throw;
                } 
                {

                }   
            }
        }
    }


    enum EventTypes
    {
        PlatformPublished,
        Undetermined
    }
}