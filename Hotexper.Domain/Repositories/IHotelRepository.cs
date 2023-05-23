using ErrorOr;
using Hotexper.Domain.Entities;

namespace Hotexper.Domain.Repositories;

public interface IHotelRepository
{
   Task<ErrorOr<Hotel>> Create(string name, string description, string? slug,  CancellationToken cancellationToken);
   Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken);
   Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
   Task<Hotel?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
}