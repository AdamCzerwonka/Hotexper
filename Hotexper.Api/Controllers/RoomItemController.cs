using Hotexper.Domain.Entities;
using Hotexper.Persistence.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/roomItem")]
public class RoomItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomItemController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomItemModel model, CancellationToken cancellationToken)
    {
        var room = new RoomItem
        {
            Id = Guid.NewGuid(),
            Number = model.Number,
            RoomId = model.RoomId
        };

        _context.RoomItems.Add(room);
        await _context.SaveChangesAsync(cancellationToken);

        return Created("test", room);
    }
}

public record CreateRoomItemModel(Guid RoomId, string Number);