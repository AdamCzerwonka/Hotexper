using Hotexper.Domain.Entities;

namespace Hotexper.Domain.Repositories;

public interface IHotelImageRepository
{
    public Task<HotelImage> Create(string altText, Guid hotelId, CancellationToken cancellationToken = default);
}