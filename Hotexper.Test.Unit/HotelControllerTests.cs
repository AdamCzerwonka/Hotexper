using FluentAssertions;
using Hotexper.Api.Controllers;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hotexper.Tests;

public class HotelControllerTests
{
    private readonly IHotelImageRepository _imageRepository = new Mock<IHotelImageRepository>().Object;
    private readonly IImageService _imageService = new Mock<IImageService>().Object;
    private readonly Mock<IHotelRepository> _hotelRepository = new();
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<ILogger<HotelController>> _logger = new();
    private readonly HotelController _sut;

    public HotelControllerTests()
    {
        _sut = new HotelController(_hotelRepository.Object, _imageService, _imageRepository, _logger.Object,
            _roleRepository.Object);
    }


    [Theory]
    [InlineData("Test Hotel", null, "test_hotel")]
    [InlineData("Test Hotel", "unique_slug", "unique_slug")]
    [InlineData("Nice Hotel", null, "nice_hotel1")]
    public async Task TestCreate_ShouldReturnHotel_WhenGivenCorrectArguments(string name, string? slug,
        string expectedSlug)
    {
        var hotels = new List<Hotel>
        {
            new() { Slug = "nice_hotel", HotelImages = new List<HotelImage>() }
        };

        _hotelRepository
            .Setup(x =>
                x.Create(
                    It.IsAny<Hotel>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Hotel hotel, CancellationToken _) =>
                {
                    hotel.HotelImages = new List<HotelImage>();
                    return hotel;
                }
            );

        _hotelRepository
            .Setup(
                x => x.GetBySlugAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (string s, CancellationToken _) => hotels.FirstOrDefault(x => x.Slug == s));

        var model = new CreateHotelModel(name, "Test", slug, "Test", "Test", "test");

        var result = (await _sut.Create(model, CancellationToken.None)).Result as CreatedResult;
        var hotel = result?.Value as HotelResponseDto;

        hotel.Should().NotBeNull();
        hotel!.Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public async Task TestCreate_ShouldFail_WhenPassedExistingSlug()
    {
        _hotelRepository
            .Setup(x =>
                x.Create(
                    It.IsAny<Hotel>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel hotel, CancellationToken _) =>
                {
                    hotel.HotelImages = new List<HotelImage>();
                    return hotel;
                }
            );

        _hotelRepository
            .Setup(
                x => x.GetBySlugAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Hotel());

        var model = new CreateHotelModel("Test Name", "Test", "nice_hotel", "Test", "Test", "Test");

        var result = (await _sut.Create(model, CancellationToken.None)).Result as UnprocessableEntityObjectResult;
        var error = result?.Value as ErrorModel;

        error.Should().NotBeNull();
        error!.StatusCode.Should().Be(422);
        error.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TestGetAll_ShouldReturnAllHotels_WhenHotelsExistsInDb()
    {
        var fakeHotels = new List<Hotel>()
        {
            new() { Id = Guid.NewGuid(), Description = "Test", Slug = "test", HotelImages = new List<HotelImage>() },
            new() { Id = Guid.NewGuid(), Description = "Test", Slug = "test", HotelImages = new List<HotelImage>() }
        };

        _hotelRepository
            .Setup(
                x => x.GetAllAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeHotels);

        var result = (await _sut.GetAll(CancellationToken.None)) as OkObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        var data = result.Value as List<HotelResponseDto>;
        data.Should().NotBeNull();
        data.Should().NotBeNullOrEmpty();
        data.Should().HaveCount(2);
    }


    [Fact]
    public async Task TestGetAll_ShouldEmptyList_WhenHotelsDoesNotExists()
    {
        _hotelRepository
            .Setup(
                x => x.GetAllAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Hotel>());

        var result = (await _sut.GetAll(CancellationToken.None)) as OkObjectResult;
        result.Should().NotBeNull();

        var data = result!.Value as List<HotelResponseDto>;
        data.Should().BeEmpty();
    }

    [Fact]
    public async Task TestGet_ShouldReturnHotel_WhenPassedExistingSlug()
    {
        var fakeHotels = new List<Hotel>
        {
            new() { Slug = "Test", HotelImages = new List<HotelImage>() }
        };

        _hotelRepository.Setup(x =>
                x.GetBySlugWithHotelImagesAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeHotels.FirstOrDefault());

        var result = (await _sut.Get("Test", CancellationToken.None)) as OkObjectResult;
        result.Should().NotBeNull();

        var data = result!.Value as HotelResponseDto;
        data.Should().NotBeNull();
    }

    [Fact]
    public async Task TestGet_ShouldReturnError_WhenHotelWithGivenSlugDoesNotExists()
    {
        _hotelRepository.Setup(
                x => x.GetBySlugWithHotelImagesAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel)null!);

        var result = await _sut.Get("Test", CancellationToken.None) as NotFoundObjectResult;
        result.Should().NotBeNull();

        var data = result!.Value as ErrorModel;
        data.Should().NotBeNull();
        data!.StatusCode.Should().Be(404);
        data.Errors.Should().NotBeNullOrEmpty();
    }
}