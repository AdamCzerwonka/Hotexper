using Microsoft.AspNetCore.Identity;

namespace Hotexper.Domain.Entities;

public class User : IdentityUser
{
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = null!;
}