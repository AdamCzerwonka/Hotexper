using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hotexper.Persistence.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<HotelImage> HotelImages => Set<HotelImage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<Room>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.HotelId);

        builder
            .Entity<Reservation>()
            .HasOne(x => x.Room)
            .WithMany(x => x.Reservations)
            .HasForeignKey(x => x.RoomId);
        
        builder
            .Entity<Reservation>()
            .HasOne(x => x.User)
            .WithMany(x => x.Reservations)
            .HasForeignKey(x => x.UserId);

        builder
            .Entity<HotelImage>()
            .HasOne(x => x.Hotel)
            .WithMany(x => x.HotelImages)
            .HasForeignKey(x => x.HotelId);
        
        base.OnModelCreating(builder);
    }
}