using System.Net.Http.Json;
using Bogus;

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5062");

await SeedUsers(client);

static async Task SeedUsers(HttpClient client)
{
    var fakeUsers = new Faker<CreateUserModel>()
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Firstname, f => f.Name.FirstName())
        .RuleFor(x => x.Lastname, f => f.Name.LastName())
        .RuleFor(x => x.Password, _ => "Test123!")
        .RuleFor(x => x.Password2, _ => "Test123!")
        .Generate(100);

    var requests = fakeUsers.Select(x => client.PostAsJsonAsync("/api/account", x));

    await Task.WhenAll(requests);
}

public class CreateUserModel
{
    public string Email { get; init; } = null!;
    public string Firstname { get; init; } = null!;
    public string Lastname { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Password2 { get; init; } = null!;
}