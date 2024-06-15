using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken ct = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException([new ValidationFailure("Rating", "Rating must be between 1 and 5.")]);
        }
        
        var movieExists = await movieRepository.ExistsByIdAsync(movieId, ct);

        if (!movieExists)
        {
            return false;
        }

        return await ratingRepository.RateMovieAsync(movieId, rating, userId, ct);
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken ct = default)
    {
        return ratingRepository.DeleteRatingAsync(movieId, userId, ct);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken ct = default)
    {
        return ratingRepository.GetRatingsForUserAsync(userId, ct);
    }
}