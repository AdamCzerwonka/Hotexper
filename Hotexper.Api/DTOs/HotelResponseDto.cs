using Hotexper.Domain.Entities;

namespace Hotexper.Api.DTOs;

public class HotelResponseDto
{
    public HotelResponseDto(Hotel hotel)
    {
        Id = hotel.Id;
        Name = hotel.Name;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
}