using System.Collections.Generic;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChanges();
        Task<IEnumerable<Platform>> GetAllPlatforms();
        Task<Platform> GetPlatformById(int id);
        void CreatePlatform(Platform plat);
    }
}