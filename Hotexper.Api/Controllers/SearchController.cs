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
        var reservations = _context
            .Reservations
            .Where(x =>
                (x.From > searchModel.From && x.From < x.To) ||
                (x.To > searchModel.From && x.To < searchModel.To)
            );

        var result = await _context
            .RoomItems
            .Where(x => reservations.All(r => x.Id == r.Id))
            .Where(x=>x.Room.Hotel.City == searchModel.Location)
            .Where(x=>x.Room.Capacity == searchModel.PeopleCount)
            .GroupBy(x => x.Room)
            .Select(x=>new {x.Key.Id, Count = x.Count()})
            .ToListAsync(cancellationToken);


        return Ok(result);
    }
}

public record SearchModel(string Location, int PeopleCount, DateTime From, DateTime To);