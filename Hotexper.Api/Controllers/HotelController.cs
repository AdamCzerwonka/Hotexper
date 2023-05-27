using Hotexper.Api.DTOs;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/hotel")]
public class HotelController : ControllerBase
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageService _imageService;
    private readonly IHotelImageRepository _hotelImageRepository;

    public HotelController(IHotelRepository hotelRepository, IImageService imageService, IHotelImageRepository hotelImageRepository)
    {
        _hotelRepository = hotelRepository;
        _imageService = imageService;
        _hotelImageRepository = hotelImageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);
        var result = hotels.Select(x => HotelResponseDto.Map(x));
        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id, cancellationToken);
        if (hotel is null)
        {
            return NotFound();
        }

        var result = HotelResponseDto.Map(hotel);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetBySlugAsync(slug, cancellationToken);
        if (hotel is null)
        {
            return NotFound();
        }

        var result = HotelResponseDto.Map(hotel);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelModel model, CancellationToken cancellationToken)
    {
        var result = await _hotelRepository.Create(model.Name, model.Description, model.Slug, cancellationToken);
        return result.Match<IActionResult>(Ok, UnprocessableEntity);
    }

    [HttpPost("{id:guid}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] Guid id, [FromForm] UploadHotelImageModel model,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id, cancellationToken);
        if (hotel is null)
        {
            return BadRequest("Hotel with given Id does not exists");
        }

        var res = await _hotelImageRepository.Create(model.AltText, hotel.Id, cancellationToken);

        await _imageService.UploadImage(res.Id.ToString(), model.File);
        return Ok();
    }
}

public record CreateHotelModel(string Name, string Description, string? Slug);

public record UploadHotelImageModel(string AltText, IFormFile File);