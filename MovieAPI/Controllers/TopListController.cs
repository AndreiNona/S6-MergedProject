using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.Service;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

 [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TopListController : ControllerBase
    {
        private readonly ITopListService _topListService;

        public TopListController(ITopListService topListService)
        {
            _topListService = topListService;
        }

        // POST: api/toplist/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateTopList([FromBody] CreateTopListRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _topListService.AddTopListAsync(userId, request.Name, request.MovieIds);
            return Ok("Top list created successfully.");
        }

        // GET: api/toplist/my-toplists
        [HttpGet("my-toplists")]
        public async Task<IActionResult> GetUserTopLists()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var topLists = await _topListService.GetUserTopListsAsync(userId);
            return Ok(topLists);
        }

        // PUT: api/toplist/{id}/update
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateTopList(int id, [FromBody] UpdateTopListRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _topListService.UpdateTopListAsync(id, userId, request.MovieIds);
            return Ok("Top list updated successfully.");
        }

        // DELETE: api/toplist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopList(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            await _topListService.DeleteTopListAsync(id, userId);
            return Ok("Top list deleted successfully.");
        }
    }

    // DTO for request
    public class CreateTopListRequest
    {
        public string Name { get; set; }
        public List<int> MovieIds { get; set; }
    }

    public class UpdateTopListRequest
    {
        public List<int> MovieIds { get; set; }
    }