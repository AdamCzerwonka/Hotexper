using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotexper.Persistence.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel> Create(Hotel hotel,
        CancellationToken cancellationToken = default)
    {
        hotel.HotelImages = new List<HotelImage>();
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(cancellationToken);

        return hotel;
    }

    public async Task<List<Hotel>> GetAllAsync(CancellationToken cancellationToken)
        => await _context
            .Hotels
            .Include(x => x.HotelImages)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context
            .Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Hotel?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
        => await _context
            .Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

    public async Task<Hotel?> GetBySlugWithHotelImagesAsync(string slug, CancellationToken cancellationToken)
        => await _context
            .Hotels
            .Include(x => x.HotelImages)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
}