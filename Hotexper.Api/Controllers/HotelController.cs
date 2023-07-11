using System.Net;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/hotel")]
public class HotelController : ControllerBase
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageService _imageService;
    private readonly IHotelImageRepository _hotelImageRepository;
    private readonly ILogger<HotelController> _logger;
    private readonly IRoleRepository _roleRepository;

    public HotelController(IHotelRepository hotelRepository, IImageService imageService,
        IHotelImageRepository hotelImageRepository,
        ILogger<HotelController> logger, IRoleRepository roleRepository)
    {
        _hotelRepository = hotelRepository;
        _imageService = imageService;
        _hotelImageRepository = hotelImageRepository;
        _logger = logger;
        _roleRepository = roleRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<HotelResponseDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);

        var result = hotels.Select(HotelResponseDto.Map).ToList();
        return Ok(result);
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

    private async Task<string> CreateHotelSlugAsync(string name, string? slug, CancellationToken cancellationToken)
    {
        if (slug is not null)
        {
            var result = await _hotelRepository.GetBySlugAsync(slug, cancellationToken);
            if (result is not null)
            {
                throw new Exception("Hotel with given slug already exists");
            }
        }
        else
        {
            Hotel? result;
            var number = 0;
            slug = name.ToLower().Replace(' ', '_');
            var startSlug = slug;
            do
            {
                slug = startSlug + (number == 0 ? "" : number);
                result = await _hotelRepository.GetBySlugAsync(slug, cancellationToken);
                number++;
            } while (result is not null);
        }

        return slug;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<HotelResponseDto>> Create([FromBody] CreateHotelModel model,
        CancellationToken cancellationToken)
    {
        string slug;
        try
        {
            slug = await CreateHotelSlugAsync(model.Name, model.Slug, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Hotel slug already exists. {Message}", e.Message);
            var error = new ErrorModel(HttpStatusCode.UnprocessableEntity, new[] { e.Message });
            return UnprocessableEntity(error);
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

        var rolesToCreate = new List<IdentityRole>
        {
            new() { Name = $"{hotel.Slug}-manager" },
            new() { Name = $"{hotel.Slug}-employee" },
        };

        await _roleRepository.CreateAsync(rolesToCreate);

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