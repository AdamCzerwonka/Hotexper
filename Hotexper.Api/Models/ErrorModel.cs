namespace Hotexper.Api.Models;

public record ErrorModel(int StatusCode, IEnumerable<string> Errors);