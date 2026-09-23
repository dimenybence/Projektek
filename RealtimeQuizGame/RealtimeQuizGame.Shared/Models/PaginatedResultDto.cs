namespace RealtimeQuizGame.Shared.Models;

public record PaginatedResultDto<T>(
    int Total,
    int PageSize,
    int CurrentPage,
    int TotalPages,
    IReadOnlyList<T> Items);
