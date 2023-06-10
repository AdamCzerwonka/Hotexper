using System.Collections;
using FluentAssertions;
using Hotexper.Api.Controllers;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Hotexper.Tests;

public class HotelControllerTests
{
    private static readonly IHotelImageRepository ImageRepository = new Mock<IHotelImageRepository>().Object;
    private static readonly IImageService ImageService = new Mock<IImageService>().Object;

    [Theory]
    [InlineData("Test Hotel", null, "test_hotel")]
    [InlineData("Test Hotel", "unique_slug", "unique_slug")]
    [InlineData("Nice Hotel", null, "nice_hotel1")]
    public async Task Test_Create_ShouldReturnHotel_WhenGivenCorrectArguments(string name, string? slug,
        string expectedSlug)
    {
        var hotelRepoMock = new Mock<IHotelRepository>();

        var hotels = new List<Hotel>
        {
            new() { Slug = "nice_hotel", HotelImages = new List<HotelImage>() }
        };

        hotelRepoMock.Setup(x =>
                x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string n, string d, string s, CancellationToken _) =>
                new Hotel() { Name = n, Description = d, Slug = s, HotelImages = new List<HotelImage>() });

        hotelRepoMock.Setup(x => x.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            (string s, CancellationToken _) => hotels.FirstOrDefault(x => x.Slug == s));

        var hotelRepo = hotelRepoMock.Object;
        var sut = new HotelController(hotelRepo, ImageService, ImageRepository);

        var model = new CreateHotelModel(name, "Test", slug);

        var result = (await sut.Create(model, CancellationToken.None)).Result as CreatedResult;
        var hotel = result?.Value as HotelResponseDto;

        hotel.Should().NotBeNull();
        hotel!.Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public async Task Test_CreateShouldFail_WhenPassedExistingSlug()
    {
        var hotelRepoMock = new Mock<IHotelRepository>();
        hotelRepoMock
            .Setup(x =>
                x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, string desc, string slug, CancellationToken _) =>
                new Hotel() { Name = name, Description = desc, Slug = slug });

        var hotels = new List<Hotel>
        {
            new() { Slug = "nice_hotel" }
        };

        hotelRepoMock.Setup(x => x.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            (string s, CancellationToken _) => hotels.FirstOrDefault(x => x.Slug == s));

        var hotelRepo = hotelRepoMock.Object;
        var sut = new HotelController(hotelRepo, ImageService, ImageRepository);

        var model = new CreateHotelModel("Test Name", "Test", "nice_hotel");

        var result = (await sut.Create(model, CancellationToken.None)).Result as UnprocessableEntityObjectResult;
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

        var hotelRepoMock = new Mock<IHotelRepository>();
        hotelRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken _) => fakeHotels);
        var sut = new HotelController(hotelRepoMock.Object, ImageService, ImageRepository);

        var result = (await sut.GetAll(CancellationToken.None)) as OkObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        var data = result.Value as List<HotelResponseDto>;
        data.Should().NotBeNull();
        data.Should().NotBeNullOrEmpty();
        data.Should().HaveCount(2);
    }


    [Fact]
    public async Task TestGetAll_ShouldReturnErorr_WhenHotelsDoesNotExists()
    {
        var fakeHotels = new List<Hotel>();

        var hotelImageRepo = new Mock<IHotelImageRepository>().Object;
        var imageService = new Mock<IImageService>().Object;
        var hotelRepoMock = new Mock<IHotelRepository>();
        hotelRepoMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken _) => fakeHotels);

        var sut = new HotelController(hotelRepoMock.Object, imageService, hotelImageRepo);

        var result = (await sut.GetAll(CancellationToken.None)) as NotFoundObjectResult;
        result.Should().NotBeNull();

        var data = result!.Value as ErrorModel;
        data.Should().NotBeNull();
        data!.StatusCode.Should().Be(404);
        data!.Errors.Should().NotBeNullOrEmpty();
    }
}