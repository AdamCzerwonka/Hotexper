using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotexper.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetAsync(CancellationToken cancellationToken)
        => await _context
            .Rooms
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Room> CreateAsync(Room room, CancellationToken cancellationToken)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync(cancellationToken);

        return room;
    }
}