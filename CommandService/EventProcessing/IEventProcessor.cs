using System.Threading.Tasks;

namespace CommandService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}