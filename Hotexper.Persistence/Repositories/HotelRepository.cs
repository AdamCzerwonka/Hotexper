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

    public async Task<Hotel> Create(string name,string description, string? slug, CancellationToken cancellationToken = default)
    {
        slug ??= name.ToLower().Replace(' ', '_');
        
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Slug = slug
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(cancellationToken);

        return hotel;
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken)
        => await _context
            .Hotels
            .AsNoTracking()
            .OrderBy(x=>x.Name)
            .ToListAsync(cancellationToken);

    public async Task<Hotel?> GetAsync(Guid id, CancellationToken cancellationToken)
        => await _context
            .Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}