using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence.Data;

namespace Hotexper.Persistence.Repositories;

public class HotelImageRepository : IHotelImageRepository
{
    private readonly AppDbContext _context;

    public HotelImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HotelImage> Create(string altText, Guid hotelId, CancellationToken cancellationToken = default)
    {
        var image = new HotelImage
        {
            Id = Guid.NewGuid(),
            AltText = altText,
            HotelId = hotelId,
            IsHidden = true
        };
        
        _context.HotelImages.Add(image);
        await _context.SaveChangesAsync(cancellationToken);

        return image;
    }
}