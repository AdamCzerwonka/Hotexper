using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class Hotel
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Description { get; set; } = null!;

    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string BuldingNumber { get; set; } = null!;

    public ICollection<Room> Rooms { get; set; } = null!;
    public ICollection<HotelImage> HotelImages { get; set; } = null!;
}