﻿using Hotexper.Domain.Entities;

namespace Hotexper.Api.DTOs;

public class HotelResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public static HotelResponseDto Map(Hotel hotel)
    {
        var hotelDto = new HotelResponseDto()
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Slug = hotel.Slug
        };

        return hotelDto;
    }
}