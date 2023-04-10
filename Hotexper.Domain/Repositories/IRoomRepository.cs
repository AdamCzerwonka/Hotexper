using Hotexper.Domain.Entities;

namespace Hotexper.Domain.Repositories;

public interface IRoomRepository
{
   Task<IEnumerable<Room>> GetAsync(CancellationToken cancellationToken);
}