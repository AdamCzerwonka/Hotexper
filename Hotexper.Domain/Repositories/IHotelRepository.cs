using Hotexper.Domain.Entities;

namespace Hotexper.Domain.Repositories;

public interface IHotelRepository
{
   Task<Hotel> Create(string name, CancellationToken cancellationToken);
   Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken);
   Task<Hotel?> GetAsync(Guid id, CancellationToken cancellationToken);
}