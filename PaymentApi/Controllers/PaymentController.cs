using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Services;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _dbContext;

        public PaymentController(IPaymentService paymentService, AppDbContext dbContext)
        {
            _paymentService = paymentService;
            _dbContext = dbContext;
        }

        // API to mark as paid, and push data to Azure Service Bus
        [Authorize]
        [HttpPost("mark-as-paid/{containerId}")]
        public async Task<IActionResult> MarkAsPaid(string containerId)
        {
            // Extract userId from JWT claims
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "User information is missing or invalid in the claims." });
            }

            // Query the database to check if the user exists
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User does not exist in the database." });
            }

            // Call PaymentService to push data to the Service Bus
            await _paymentService.PushDataToServiceBusAsync(containerId);

            return Ok(new { message = "Container marked as paid, notification sent to Service Bus." });
        }

        // API to process Service Bus messages, update SQL and Cosmos DB
        [Authorize]
        [HttpPost("process-service-bus-message")]
        public async Task<IActionResult> ProcessServiceBusMessage()
        {
            // Extract userId from JWT claims
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "User information is missing or invalid in the claims." });
            }

            // Call PaymentService to receive and process the message
            await _paymentService.ProcessMessageFromServiceBusAsync(userId);

            return Ok("Service bus message processed.");
        }
    }
}
