using Microsoft.AspNetCore.Identity;

namespace Hotexper.Domain.Entities;

public class User : IdentityUser
{
    public ICollection<Reservation> Reservations { get; set; } = null!;
}