using System.Net;
using FluentAssertions;

namespace Hotexper.Test.Integration;

public class AccountControllerTests:IntegrationTest
{
   [Fact]
   public async Task TestGet_ShouldReturnNotFound_WhenUserDoesNotExists()
   {
       var userId = Guid.NewGuid();

       var result = await HttpClient.GetAsync($"/api/account/{userId}");
       result.StatusCode.Should().Be(HttpStatusCode.NotFound);
   } 
}