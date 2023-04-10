using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[Route("/api/hotel")]
public class HotelController : ControllerBase
{
    private readonly IHotelRepository _hotelRepository;

    public HotelController(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);
        return Ok(hotels);
    }

    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetAsync(id, cancellationToken);
        if (hotel is null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelModel model, CancellationToken cancellationToken)
    {
        var result = await _hotelRepository.Create(model.Name, cancellationToken);
        return Ok(result);
    }
}

public record CreateHotelModel(string Name);