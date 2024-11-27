using Microsoft.AspNetCore.Mvc;
using PaymentApi.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, AppDbContext dbContext, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("mark-as-paid/{containerId}")]
        public async Task<IActionResult> MarkAsPaid(string containerId)
        {
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user information.");
                    return Unauthorized(new { message = "Invalid user information." });
                }

                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    return Unauthorized(new { message = "User not found." });
                }

                await _paymentService.PushDataToServiceBusAsync(containerId);
                return Ok(new { message = "Container marked as paid, notification sent to Service Bus." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while marking container {containerId} as paid: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [Authorize]
        [HttpPost("process-service-bus-message")]
        public async Task<IActionResult> ProcessServiceBusMessage()
        {
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user information.");
                    return Unauthorized(new { message = "Invalid user information." });
                }

                await _paymentService.ProcessMessageFromServiceBusAsync(userId);
                return Ok(new { message = "Service bus message processed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing service bus message: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing the message." });
            }
        }
    }
}
