using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken ct = default);
    
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken ct = default);
    
    Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken ct = default);
}