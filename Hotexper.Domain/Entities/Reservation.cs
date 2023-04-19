using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class Reservation
{
    [Key]
    public Guid Id { get; set; }

    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}