namespace RestaurantService.Application.Common;

public enum ServiceStatus
{
    Success,
    NotFound,
    Forbidden
}

/// <summary>
/// Outcome of a mutating application-service operation that can fail either
/// because the target doesn't exist (404) or because the caller isn't allowed
/// to act on it (403), in addition to the happy path.
/// </summary>
public class ServiceResult
{
    public ServiceStatus Status { get; init; }

    public static ServiceResult Success() => new() { Status = ServiceStatus.Success };

    public static ServiceResult NotFound() => new() { Status = ServiceStatus.NotFound };

    public static ServiceResult Forbidden() => new() { Status = ServiceStatus.Forbidden };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; init; }

    public static ServiceResult<T> Success(T value) => new() { Status = ServiceStatus.Success, Value = value };

    public static new ServiceResult<T> NotFound() => new() { Status = ServiceStatus.NotFound };

    public static new ServiceResult<T> Forbidden() => new() { Status = ServiceStatus.Forbidden };
}
