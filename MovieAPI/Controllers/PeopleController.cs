using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.Service;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController :ControllerBase
{
    private readonly IPeopleService _peopleService;

    public PeopleController(IPeopleService peopleService)
    {
        _peopleService = peopleService;
    }
    // GET: api/people/name/{name}
    [HttpGet("name/{name}")]
    public async Task<ActionResult<IEnumerable<Person>>> GetPersonByName(string name, [FromQuery] int maxResults = 10)
    {
        var people = await _peopleService.GetPeopleByName(name, maxResults);

        if (!people.Any())
        {
            return NotFound("No person found with the given name.");
        }

        return Ok(people);
    }
    
// GET: api/people/{personId}/starred?includeOmdbDetails=true
    [HttpGet("{personId}/starred")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesStarredInByPerson(int personId, [FromQuery] bool includeOmdbDetails = false)
    {
        var movies = await _peopleService.GetMoviesStarredInByPerson(personId, includeOmdbDetails);

        if (!movies.Any())
        {
            return NotFound("No movies found where the person is a star.");
        }

        return Ok(movies);
    }

    // GET: api/people/{personId}/directed
    [HttpGet("{personId}/directed")]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesDirectedByPerson(int personId)
    {
        var movies = await _peopleService.GetMoviesDirectedByPerson(personId);

        if (!movies.Any())
        {
            return NotFound("No movies found where the person is a director.");
        }

        return Ok(movies);
    }
    
// GET: api/people/{personId}/role
    [HttpGet("{personId}/role")]
    public async Task<ActionResult> GetPersonRole(int personId)
    {
        var result = await _peopleService.GetPersonRole(personId);

        if (result == null)
        {
            return NotFound("No movies found for the person.");
        }

        return Ok(result);
    }
}