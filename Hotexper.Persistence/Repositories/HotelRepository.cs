using ErrorOr;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotexper.Persistence.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<HotelRepository> _logger;

    public HotelRepository(AppDbContext context, ILogger<HotelRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Hotel>> Create(string name, string description, string? slug,
        CancellationToken cancellationToken = default)
    {
        if (slug is not null)
        {
            var result = await GetBySlugAsync(slug, cancellationToken);
            if (result is not null)
            {
                return Error.Failure(description: "Hotel with given slug already exists");
            }
        }
        else
        { 
            _logger.LogInformation("Slug not passed! Generating slug.");
            Hotel? result = null;
            var number = 0;
            slug = name.ToLower().Replace(' ', '_');
            var startSlug = slug;
            do
            {
                slug = startSlug + (number == 0 ? "" : number);
                _logger.LogInformation("Testing slug: '{slug}'", slug);
                result = await GetBySlugAsync(slug, cancellationToken);
                number++;
            } while (result is not null);
        }

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
}