namespace Hotexper.Api.Models;

public record CreateUserModel(string Email, string Firstname, string Lastname, string PhoneNumber, string Password,
    string Password2);