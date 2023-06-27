using Hotexper.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Hotexper.Api.DTOs;

public class RoomResponseDto
{
    public RoomResponseDto(Room room)
    {
        Id = room.Id;
        HotelId = room.HotelId;
    }

    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
}