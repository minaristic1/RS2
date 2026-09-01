using System.ComponentModel.DataAnnotations;

namespace Billing.API.DTOs;

public sealed record CreateInvoiceRequest(
    [Required] Guid OrderId,
    [Required] Guid RestaurantId,
    [Required, StringLength(500)] string DeliveryAddress,
    [Required, StringLength(3, MinimumLength = 3)] string Currency,
    [Required, MinLength(1)] IReadOnlyCollection<CreateInvoiceItemRequest> Items);

public sealed record CreateInvoiceItemRequest(
    [Required] Guid ProductId,
    [Required, StringLength(200)] string Name,
    [Range(1, int.MaxValue)] int Quantity,
    [Range(typeof(decimal), "0.01", "9999999999999999")] decimal UnitPrice);
