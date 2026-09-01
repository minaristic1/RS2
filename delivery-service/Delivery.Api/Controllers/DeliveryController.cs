using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Delivery.Data.Models;
using Delivery.Api.Features.Deliveries.Commands.CreateDeliveryOrder;
using Delivery.Api.Features.Deliveries.Commands.AdvanceDeliveryStatus;
using Delivery.Api.Features.Deliveries.Commands.CancelDelivery;
using Delivery.Api.Features.Deliveries.Commands.AssignCourier;
using Delivery.Api.Features.Deliveries.Queries.GetDeliveryById;
using Delivery.Api.Features.Deliveries.Queries.GetDeliveryByOrderId;
using Delivery.Api.Features.Deliveries.Queries.GetAllDeliveries;

namespace Delivery.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DeliveryOrder>> Create(CreateDeliveryOrderCommand command)
        {
            var delivery = await _mediator.Send(command);
            return Created($"/api/delivery/{delivery.Id}", delivery);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryOrder>> GetById(Guid id)
        {
            var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<ActionResult<DeliveryOrder>> GetByOrderId(Guid orderId)
        {
            var delivery = await _mediator.Send(new GetDeliveryByOrderIdQuery(orderId));

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }

        [HttpGet]
        public async Task<ActionResult<List<DeliveryOrder>>> GetAll([FromQuery] DeliveryStatus? status)
        {
            var deliveries = await _mediator.Send(new GetAllDeliveriesQuery(status));

            return Ok(deliveries);
        }

        [HttpPost("{id}/advance-status")]
        [Authorize]
        public async Task<ActionResult<DeliveryOrder>> AdvanceStatus(Guid id)
        {
            try
            {
                var delivery = await _mediator.Send(new AdvanceDeliveryStatusCommand(id));

                if (delivery == null)
                {
                    return NotFound();
                }

                return Ok(delivery);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<DeliveryOrder>> Cancel(Guid id)
        {
            try
            {
                var delivery = await _mediator.Send(new CancelDeliveryCommand(id));

                if (delivery == null)
                {
                    return NotFound();
                }

                return Ok(delivery);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/assign-courier")]
        [Authorize]
        public async Task<ActionResult<DeliveryOrder>> AssignCourier(Guid id, [FromQuery] Guid courierId)
        {
            var delivery = await _mediator.Send(new AssignCourierCommand(id, courierId));

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }
    }
}
