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
    public IActionResult GetTest()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelModel model, CancellationToken cancellationToken)
    {
        var result = await _hotelRepository.Create(model.Name, cancellationToken);
        return Ok(result);
    }
}

public record CreateHotelModel(string Name);