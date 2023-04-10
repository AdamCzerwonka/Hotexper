using Hotexper.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Hotexper.Api.DTOs;

public class RoomResponseDto
{
    public RoomResponseDto(Room room)
    {
        Id = room.Id;
        Number = room.Number;
        HotelId = room.HotelId;
    }

    public Guid Id { get; set; }
    public string Number { get; set; }
    public Guid HotelId { get; set; }
}