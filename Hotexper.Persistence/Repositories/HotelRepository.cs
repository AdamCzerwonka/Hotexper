using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence.Data;

namespace Hotexper.Persistence.Repositories;

public class HotelRepository: IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Hotel> Create(string name, CancellationToken cancellationToken = default)
    {
        var hotel = new Hotel
        {
            Name = name,
            Id = Guid.NewGuid()
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(cancellationToken);

        return hotel;
    }
}