namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken ct = default);
    
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken ct = default);

    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken ct = default);
    
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken ct = default);
}