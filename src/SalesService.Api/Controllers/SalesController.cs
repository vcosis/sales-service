using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesService.Application.Commands;
using SalesService.Application.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all sales with pagination, filtering, and ordering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SaleDto>>> GetSales(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? order = null,
        [FromQuery] string? filter = null)
    {
        var query = new GetSalesQuery
        {
            Page = page,
            Size = size,
            Order = order,
            Filter = filter
        };

        var sales = await _mediator.Send(query);
        return Ok(sales);
    }

    /// <summary>
    /// Get a specific sale by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetSale(int id)
    {
        var query = new GetSaleByIdQuery(id);
        var sale = await _mediator.Send(query);
        return Ok(sale);
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SaleDto>> CreateSale([FromBody] CreateSaleCommand command)
    {
        var sale = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
    }

    /// <summary>
    /// Update an existing sale
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SaleDto>> UpdateSale(int id, [FromBody] UpdateSaleCommand command)
    {
        command.Id = id;
        var sale = await _mediator.Send(command);
        return Ok(sale);
    }

    /// <summary>
    /// Cancel a sale
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<SaleDto>> CancelSale(int id)
    {
        var command = new CancelSaleCommand(id);
        var sale = await _mediator.Send(command);
        return Ok(sale);
    }
} 