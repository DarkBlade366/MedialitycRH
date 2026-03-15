using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IRedmineSyncJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
