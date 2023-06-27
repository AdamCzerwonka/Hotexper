using ErrorOr;
using Hotexper.Domain.Entities;

namespace Hotexper.Domain.Repositories;

public interface IHotelRepository
{
    Task<Hotel> Create(Hotel hotel, CancellationToken cancellationToken);
    Task<List<Hotel>> GetAllAsync(CancellationToken cancellationToken);
    Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Hotel?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<Hotel?> GetBySlugWithHotelImagesAsync(string slug, CancellationToken cancellationToken);
}