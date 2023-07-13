using System.Net;

namespace Hotexper.Api.Models;

public record Error(int StatusCode, IEnumerable<string> Errors)
{
    public Error(HttpStatusCode code, IEnumerable<string> Errors) : this((int)code, Errors)
    {
    }

    public static Error BadRequestError(IEnumerable<string> errors)
        => new(StatusCodes.Status400BadRequest, errors);

    public static Error UnprocessableEntityError(IEnumerable<string> errors)
        => new(StatusCodes.Status422UnprocessableEntity, errors);
}