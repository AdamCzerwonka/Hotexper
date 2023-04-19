using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class Room
{
    [Key]
    public Guid Id { get; set; }

    public string Number { get; set; } = null!;

    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = null!;
}