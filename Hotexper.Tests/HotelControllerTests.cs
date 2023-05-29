using FluentAssertions;
using Hotexper.Api.Controllers;
using Hotexper.Api.DTOs;
using Hotexper.Api.Models;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hotexper.Tests;

public class HotelControllerTests
{
    [Theory]
    [InlineData("Test Hotel", null, "test_hotel")]
    [InlineData("Test Hotel", "unique_slug", "unique_slug")]
    [InlineData("Nice Hotel", null, "nice_hotel1")]
    public async Task Test_Create_ShouldReturnHotel_WhenGivenCorrectArguments(string name, string? slug,
        string expectedSlug)
    {
        var hotelImageRepo = new Mock<IHotelImageRepository>().Object;
        var imageService = new Mock<IImageService>().Object;
        var hotelRepoMock = new Mock<IHotelRepository>();

        var hotels = new List<Hotel>
        {
            new() { Slug = "nice_hotel" }
        };

        hotelRepoMock.Setup(x =>
                x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string n, string d, string s, CancellationToken _) =>
                new Hotel() { Name = n, Description = d, Slug = s });

        hotelRepoMock.Setup(x => x.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            (string s, CancellationToken _) => hotels.FirstOrDefault(x => x.Slug == s));

        var hotelRepo = hotelRepoMock.Object;
        var sut = new HotelController(hotelRepo, imageService, hotelImageRepo);

        var model = new CreateHotelModel(name, "Test", slug);

        var result = (await sut.Create(model, CancellationToken.None)).Result as CreatedResult;
        var hotel = result?.Value as HotelResponseDto;

        hotel.Should().NotBeNull();
        hotel!.Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public async Task Test_CreateShouldFail_WhenPassedExistingSlug()
    {
        var hotelImageRepo = new Mock<IHotelImageRepository>().Object;
        var imageService = new Mock<IImageService>().Object;
        var hotelRepoMock = new Mock<IHotelRepository>();
        hotelRepoMock.Setup(x =>
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
        var sut = new HotelController(hotelRepo, imageService, hotelImageRepo);

        var model = new CreateHotelModel("Test Name", "Test", "nice_hotel");

        var result = (await sut.Create(model, CancellationToken.None)).Result as UnprocessableEntityObjectResult;
        var error = result?.Value as ErrorModel;

        error.Should().NotBeNull();
        error!.StatusCode.Should().Be(422);
        error.Errors.Should().NotBeEmpty();
    }
}