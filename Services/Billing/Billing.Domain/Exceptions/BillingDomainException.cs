namespace Billing.Domain.Exceptions;

public sealed class BillingDomainException(string message) : Exception(message);

