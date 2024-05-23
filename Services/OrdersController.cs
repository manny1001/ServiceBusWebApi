using Microsoft.AspNetCore.Mvc;
using ServiceBusWebApi.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ServiceBusSenderService _serviceBusSenderService;

    public OrdersController(ServiceBusSenderService serviceBusSenderService)
    {
        _serviceBusSenderService = serviceBusSenderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] string order)
    {
        await _serviceBusSenderService.SendMessageAsync(order);
        return Ok("Order sent to Service Bus.");
    }
}
