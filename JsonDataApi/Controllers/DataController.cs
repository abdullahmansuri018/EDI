using JsonDataApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [Authorize]
        [HttpGet("fetch-by-containerId/{containerId}")]
        public async Task<IActionResult> FetchDataByContainerId(string containerId)
        {
            // Extract userId and email from JWT Claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            bool isPaid=false;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("User information is missing from the claims.");
            }

            var data = await _dataService.FetchDataByContainerId(containerId, userId, email,isPaid);

            if (data == null)
            {
                return NotFound($"No data found for ContainerId: {containerId}");
            }

            return Ok(data);
        }

        [Authorize]
        [HttpGet("fetch-by-userId")]
        public async Task<IActionResult> FetchDataByUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User information is missing from the claims.");
            }

            var data = await _dataService.FetchDataByUserId(userId);
            return Ok(data);
        }

        [Authorize]
        [HttpDelete("remove-container/{containerId}")]
        public async Task<IActionResult> RemoveContainer(string containerId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User information is missing from the claims.");
            }

            var success = await _dataService.RemoveContainer(containerId, userId);

            if (!success)
            {
                return NotFound($"Container with ID {containerId} not found or you do not have permission to delete it.");
            }

            return NoContent();  // Return 204 No Content on successful deletion
        }
    }
}
