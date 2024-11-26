using JsonDataApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

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
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { message = "User information is missing from the claims." });
                }

                var data = await _dataService.FetchDataByContainerId(containerId, userId, email);

                if (data == null)
                {
                    return NotFound(new { message = $"No data found for ContainerId: {containerId}" });
                }

                return Ok(data);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("fetch-by-userId")]
        public async Task<IActionResult> FetchDataByUserId()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User information is missing from the claims." });
                }

                var data = await _dataService.FetchDataByUserId(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("remove-container/{containerId}")]
        public async Task<IActionResult> RemoveContainer(string containerId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User information is missing from the claims." });
                }

                var success = await _dataService.RemoveContainer(containerId, userId);

                if (!success)
                {
                    return NotFound(new { message = $"Container with ID {containerId} not found or you do not have permission to delete it." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
