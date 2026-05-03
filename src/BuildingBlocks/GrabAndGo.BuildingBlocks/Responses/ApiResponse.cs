namespace GrabAndGo.BuildingBlocks.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<ApiError>? Errors { get; set; }
    public string? TraceId { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string? message = null, string? traceId = null) =>
        new() { Success = true, Data = data, Message = message, TraceId = traceId };

    public static ApiResponse<T> FailureResult(string message, List<ApiError>? errors = null, string? traceId = null) =>
        new() { Success = false, Message = message, Errors = errors, TraceId = traceId };
}

public class ApiError
{
    public string? Field { get; set; }
    public string Message { get; set; } = string.Empty;
}
