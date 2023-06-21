using System.Net;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
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

    public HotelController(IHotelRepository hotelRepository, IImageService imageService,
        IHotelImageRepository hotelImageRepository)
    {
        _hotelRepository = hotelRepository;
        _imageService = imageService;
        _hotelImageRepository = hotelImageRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<HotelResponseDto>), 200)]
    [ProducesResponseType(typeof(ErrorModel), 404)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);

        if (hotels.Any())
        {
            var result = hotels.Select(HotelResponseDto.Map).ToList();
            return Ok(result);
        }

        var error = new ErrorModel(HttpStatusCode.NotFound, new[] { "No hotels found" });
        return NotFound(error);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(HotelResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorModel), 404)]
    public async Task<IActionResult> Get(string slug, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetBySlugWithHotelImagesAsync(slug, cancellationToken);

        if (hotel is not null)
        {
            var hotelDto = HotelResponseDto.Map(hotel);
            return Ok(hotelDto);
        }

        var error = new ErrorModel(HttpStatusCode.NotFound, new[] { "Hotel with given slug does not exists" });
        return NotFound(error);
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<HotelResponseDto>> Create([FromBody] CreateHotelModel model,
        CancellationToken cancellationToken)
    {
        string slug;
        if (model.Slug is not null)
        {
            slug = model.Slug;
            var result = await _hotelRepository.GetBySlugAsync(slug, cancellationToken);
            if (result is not null)
            {
                return UnprocessableEntity(new ErrorModel(HttpStatusCode.UnprocessableEntity,
                    new[] { "Hotel with given slug already exists." }));
            }
        }
        else
        {
            Hotel? result;
            var number = 0;
            slug = model.Name.ToLower().Replace(' ', '_');
            var startSlug = slug;
            do
            {
                slug = startSlug + (number == 0 ? "" : number);
                result = await _hotelRepository.GetBySlugAsync(slug, cancellationToken);
                number++;
            } while (result is not null);
        }

        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Slug = slug,
            City = model.City,
            Street = model.Street,
            BuldingNumber = model.BuildingNumber
        };


        var res = await _hotelRepository.Create(hotel, cancellationToken);
        return Created("test", HotelResponseDto.Map(res));
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

public record CreateHotelModel(string Name, string Description, string? Slug, string City, string Street,
    string BuildingNumber);

public record UploadHotelImageModel(string AltText, IFormFile File);