using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController :ControllerBase
{
    private readonly MovieDbContext _context;

    public PeopleController(MovieDbContext context)
    {
        _context = context;
    }
    // GET: api/people/name/{name}
    [HttpGet("name/{name}")]
    public async Task<ActionResult<IEnumerable<Person>>> GetPersonByName(string name, [FromQuery] int maxResults = 10)
    {
        var people = await _context.People
            .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
            .Take(maxResults)
            .ToListAsync();

        if (people == null || !people.Any())
        {
            return NotFound("No person found with the given name.");
        }

        return Ok(people);
    }
}