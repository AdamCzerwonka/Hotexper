using System.ComponentModel.DataAnnotations;

namespace Hotexper.Domain.Entities;

public class HotelImage
{
    [Key]
    public Guid Id { get; set; }
    public string AltText { get; set; } = null!;
    public bool IsHidden { get; set; } = true;
    public Guid HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;
}