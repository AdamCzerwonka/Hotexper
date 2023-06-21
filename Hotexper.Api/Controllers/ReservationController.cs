using Hotexper.Domain.Entities;
using Hotexper.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Hotexper.Api.Controllers;

[Route("/api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReservationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var reservations = await _context.Reservations.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(reservations);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReservationModel reservationModel,
        CancellationToken cancellationToken)
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
        var reservation = new Reservation
        {
            RoomId = reservationModel.RoomId,
            From = reservationModel.From,
            To = reservationModel.To,
            UserId = userId
        };

        var roomExists = await _context.RoomItems.AsNoTracking()
            .AnyAsync(x => x.Id == reservationModel.RoomId, cancellationToken);

        if (!roomExists)
        {
            return BadRequest("Room with given id does not exists");
        }

        var canCreate = await _context
            .Reservations
            .AsNoTracking()
            .Where(x => x.RoomId == reservation.RoomId)
            .AnyAsync(x =>
                    (x.From > reservation.From && x.From < x.To) ||
                    (x.To > reservation.From && x.To < reservation.To)
                , cancellationToken);

        if (canCreate)
        {
            return BadRequest();
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(reservation);
    }
}

public record CreateReservationModel(Guid RoomId, DateTime From, DateTime To);