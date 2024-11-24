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
        private readonly AppDbContext _dbContext;  // Inject your DbContext to interact with the database

        public PaymentController(IPaymentService paymentService, AppDbContext dbContext)
        {
            _paymentService = paymentService;
            _dbContext = dbContext;
        }

        // API to mark as paid, using containerId in the URL
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

            // Call PaymentService to mark the container as paid and send a message to Azure Service Bus
            var result = await _paymentService.MarkAsPaidAndNotifyAsync(userId, containerId);

            if (result)
            {
                try
                {
                    // Call PaymentService to update the 'Holds' field in Cosmos DB
                    await _paymentService.UpdateContainerInCosmosDb(containerId);
                    return Ok(new { message = "Container marked as paid, notification sent, and Holds field updated." });
                }
                catch (Exception ex)
                {
                    // Handle Cosmos DB update failure
                    return NotFound(new { message = "Container not found in Cosmos DB or failed to update.", error = ex.Message });
                }
            }
            else
            {
                return NotFound(new { message = "Container not found or user does not have permission." });
            }
        }


        // API to receive and process messages from the Service Bus
        [Authorize]
        [HttpPost("process-service-bus-message")]
        public async Task<IActionResult> ProcessServiceBusMessage()
        {
            await _paymentService.ReceiveAndProcessMessageAsync();
            return Ok("Service bus message processed.");
        }
    }
}
