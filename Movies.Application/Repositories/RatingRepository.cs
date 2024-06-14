using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository(IDbConnectionFactory dbConnectionFactory) : IRatingRepository
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                INSERT INTO ratings (movieId, userId, rating)
                VALUES (@MovieId, @UserId, @Rating)
                ON CONFLICT (movieId, userId) DO UPDATE
                SET rating = @Rating;
                """,
                new { MovieId = movieId, UserId = userId, Rating = rating },
                cancellationToken: ct
            )
        );
        
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        return await connection.QuerySingleOrDefaultAsync<float?>(
            new CommandDefinition(
                """
                SELECT ROUND(AVG(r.rating), 1)
                FROM ratings r
                WHERE r.movieId = @MovieId;
                """,
                new { MovieId = movieId },
                cancellationToken: ct
            )
        );
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(
            new CommandDefinition(
                """
                SELECT ROUND(AVG(r.rating), 1) AS Rating, myr.rating AS UserRating
                FROM ratings r
                LEFT JOIN ratings myr ON r.movieId = myr.movieId AND myr.userId = @UserId
                WHERE r.movieId = @MovieId
                GROUP BY myr.rating;
                """,
                new { MovieId = movieId, UserId = userId },
                cancellationToken: ct
            )
        );
    }
}