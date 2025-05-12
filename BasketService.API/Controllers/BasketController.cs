using BasketService.Application.Commands;
using BasketService.Application.DTOs;
using BasketService.Application.Queries;
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
        var basketId = await _mediator.Send(new CreateBasketCommand { UserId = userId });
        return Ok(new { BasketId = basketId });

    }
    [HttpPost("{userId}/add")]
    public async Task<IActionResult> AddProduct(Guid userId, [FromBody] AddProductDto request)
    {
        await _mediator.Send(new AddProductCommand
            { UserId = userId, ProductId = request.ProductId, Quantity = request.Quantity });
        return Ok();
    }
    
    [HttpPost("{userId}/remove")]
    public async Task<IActionResult> RemoveProduct(Guid userId, [FromBody] RemoveProductDto request)
    {
        return Ok(await _mediator.Send(new RemoveProductCommand(){UserId = userId, ProductId = request.ProductId, Quantity =  request.Quantity}));
    }
    
    [HttpPost("{userId}/finalize")]
    public async Task<IActionResult> Finalize(Guid userId)
    {
        return Ok(await _mediator.Send(new FinalizeBasketCommand() { UserId = userId }));
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBasket(Guid userId)
    {
        var result = await _mediator.Send(new GetBasketByIdQuery(userId));
        return Ok(result);
    }
    
    
}