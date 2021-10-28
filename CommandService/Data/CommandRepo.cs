using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if(command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;   
            _context.Commands.Add(command);
        }
        public void CreatePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            _context.Platforms.Add(plat);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Command> GetCommand(int platformId, int commandId)
        {
            return await _context.Commands
                .Where(c => c.Id == commandId && c.PlatformId == platformId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId)
        {
            return await _context.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name)
                .ToListAsync();
        }
        public async Task<bool> PlatformExists(int platofrmId)
        {
            return await _context.Platforms.AnyAsync(p => p.Id == platofrmId);
        }

        public async Task<bool> SaveChanges()
        {
            return ( await _context.SaveChangesAsync() )> 0;
        }
        public async Task<bool> ExternalPlatformExists(int externalPlatformId)
        {
            return await _context.Platforms.AnyAsync(
                p => p.ExternalID == externalPlatformId
            );
        }
    }
}