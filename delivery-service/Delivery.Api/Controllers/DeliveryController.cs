using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery.Api.Data;
using Delivery.Api.Models;
using Delivery.Api.DTOs;

namespace Delivery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public DeliveryController(DeliveryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryOrder>> Create(CreateDeliveryOrderRequest request)
        {
            var delivery = new DeliveryOrder(
                request.OrderId,
                request.CustomerName,
                request.CustomerPhone,
                request.RestaurantId,
                request.RestaurantName,
                request.PickupAddress,
                request.DeliveryAddress,
                request.TotalPrice
            );

            delivery.Items = request.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return Created($"/api/delivery/{delivery.Id}", delivery);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryOrder>> GetById(Guid id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }

        [HttpGet]
        public async Task<ActionResult<List<DeliveryOrder>>> GetAll()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Items)
                .ToListAsync();

            return Ok(deliveries);
        }

        [HttpPost("{id}/advance-status")]
        public async Task<ActionResult<DeliveryOrder>> AdvanceStatus(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            try
            {
                delivery.AdvanceStatus();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();

            return Ok(delivery);
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<DeliveryOrder>> Cancel(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            try
            {
                delivery.Cancel();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();

            return Ok(delivery);
        }
    }
}
