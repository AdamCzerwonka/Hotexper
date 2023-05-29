using System.Net;

namespace Hotexper.Api.Models;

public record ErrorModel(int StatusCode, IEnumerable<string> Errors)
{
    public ErrorModel(HttpStatusCode code, IEnumerable<string> Errors) : this((int)code, Errors)
    {
    }
}