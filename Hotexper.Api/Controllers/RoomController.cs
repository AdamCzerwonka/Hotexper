using Hotexper.Api.DTOs;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[Route("/api/room")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetAsync(cancellationToken);
        var result = rooms.Select(x => new RoomResponseDto(x));

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomModel createRoomModel,
        CancellationToken cancellationToken)
    {
        var room = new Room
        {
           HotelId = createRoomModel.HotelId,
           Number = createRoomModel.RoomNumber
        };

        await _roomRepository.CreateAsync(room, cancellationToken);

        var result = new RoomResponseDto(room);
        return Ok(result);
    }
}

public record CreateRoomModel(Guid HotelId, string RoomNumber);