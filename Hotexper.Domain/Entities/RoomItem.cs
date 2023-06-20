using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class RoomItem
{
    [Key]
    public Guid Id { get; set; }

    public string Number { get; set; } = null!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public ICollection<Reservation> Reservations = null!;
}