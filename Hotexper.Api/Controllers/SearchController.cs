using Hotexper.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotexper.Api.Controllers;

[ApiController]
[Route("/api/search")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<SearchController> _logger;

    public SearchController(AppDbContext context, ILogger<SearchController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SearchModel searchModel, CancellationToken cancellationToken)
    {
        var result = await _context.Rooms
            .AsNoTracking()
            .Where(x => x.Hotel.City == searchModel.Location)
            .Where(x => x.Capacity == searchModel.PeopleCount)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}

public record SearchModel(string Location, int PeopleCount, DateOnly From, DateOnly To);