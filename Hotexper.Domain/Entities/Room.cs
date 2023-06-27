using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class Room
{
    [Key]
    public Guid Id { get; set; }
    public int Standard { get; set; }
    public int Capacity { get; set; }
    public float Price { get; set; }
    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public ICollection<RoomItem> RoomItems { get; set; } = null!;
}