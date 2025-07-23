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
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="size">Number of items per page (default: 10)</param>
    /// <param name="order">Ordering criteria (e.g., "date desc", "customername asc")</param>
    /// <param name="filter">Filter criteria (e.g., "cancelled=true", "customername=John*")</param>
    /// <remarks>
    /// Filter examples:
    /// - cancelled=true (show only cancelled sales)
    /// - cancelled=false (show only active sales)
    /// - customername=John* (sales with customer name starting with "John")
    /// - salenumber=SALE* (sales with number starting with "SALE")
    /// - totalamount=1500 (sales with exact total amount)
    /// - totalamount>1000 (sales with total amount greater than 1000)
    /// - totalamount&lt;5000 (sales with total amount less than 5000)
    /// - totalamount=1000-5000 (sales with total amount between 1000 and 5000)
    /// - Multiple filters: cancelled=true&amp;totalamount>1000
    /// - Multiple filters: cancelled=false&amp;customername=John*&amp;totalamount&lt;2000
    /// </remarks>
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