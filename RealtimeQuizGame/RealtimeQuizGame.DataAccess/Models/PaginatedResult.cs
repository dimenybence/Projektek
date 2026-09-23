namespace RealtimeQuizGame.DataAccess.Models;

public record PaginatedResult<T>(
    int Total,
    int PageSize,
    int CurrentPage,
    int TotalPages,
    IReadOnlyList<T> Items);
