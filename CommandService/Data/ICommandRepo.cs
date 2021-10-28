using System.Collections.Generic;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        Task<bool> SaveChanges();

        //for platforms
        Task<IEnumerable<Platform>> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        Task<bool> PlatformExists(int platofrmId);
        //checks if platform exists given an external id
        Task<bool> ExternalPlatformExists(int externalPlatformId);
        //for commands
        void CreateCommand(int platformId, Command command);
        Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId);
        Task<Command> GetCommand(int platformId, int commandId);
    }
}