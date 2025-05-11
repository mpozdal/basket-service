using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.API.Controllers;
[ApiController]
[Route("api/basket")]
public class BasketController: ControllerBase
{
    private readonly  IMediator _mediator;

    public BasketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> CreateBasket(Guid userId)
    {
        return Ok();
    }
    
    
}