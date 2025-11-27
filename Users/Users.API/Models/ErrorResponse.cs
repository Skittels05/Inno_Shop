namespace Users.Api.Models;

public record ErrorResponse(
    bool Error,
    string Message,
    IDictionary<string, string[]>? Errors = null,
    string? TraceId = null
);