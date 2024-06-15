using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO movies (id, title, slug, yearofrelease)
            VALUES (@Id, @Title, @Slug, @YearOfRelease);
            """,
            movie,
            transaction,
            cancellationToken: ct
        ));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    """
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);
                    """,
                    new { MovieId = movie.Id, Name = genre },
                    transaction,
                    cancellationToken: ct
                ));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                SELECT m.*, ROUND(AVG(r.rating), 1) AS rating, myr.rating AS userrating
                FROM movies m
                LEFT JOIN ratings r ON m.id = r.movieId
                LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
                WHERE id = @Id
                GROUP BY m.id, myr.rating;
                """,
                new { Id = id, UserId = userId })
        );
        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                SELECT name
                FROM genres
                WHERE movieId = @MovieId;
                """,
                new { MovieId = id },
                cancellationToken: ct)
        );

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(
            """
            SELECT m.*, ROUND(AVG(r.rating), 1) AS rating, myr.rating AS userrating
            FROM movies m
            LEFT JOIN ratings r ON m.id = r.movieId
            LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
            WHERE slug = @Slug
            GROUP BY m.id, myr.rating;
            """,
            new { Slug = slug, UserId = userId },
            cancellationToken: ct)
        );
        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                SELECT name
                FROM genres
                WHERE movieId = @MovieId;
                """,
                new { MovieId = movie.Id },
                cancellationToken: ct)
        );

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);

        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $", ORDER BY {options.SortField} {(options.SortOrder == SortOrder.Descending ? "DESC" : "ASC")}";
        }
        
        var result = await connection.QueryAsync(
            new CommandDefinition($"""
                                  SELECT m.*,
                                         STRING_AGG(DISTINCT g.name, ', ') AS genres,
                                         ROUND(AVG(r.rating), 1) AS rating,
                                         myr.rating AS userrating
                                  FROM movies m
                                  LEFT JOIN genres g ON m.id = g.movieId
                                  LEFT JOIN ratings r ON m.id = r.movieId
                                  LEFT JOIN ratings myr ON m.id = myr.movieId AND myr.userId = @UserId
                                  WHERE (@Title IS NULL OR m.title LIKE ('%' || @Title || '%'))
                                  AND (@YearOfRelease IS NULL OR m.yearOfRelease = @YearOfRelease)
                                  GROUP BY m.id, myr.rating {orderClause}
                                  """,
                new { options.UserId, options.Title, options.YearOfRelease },
                cancellationToken: ct)
        );

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(", "))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM genres WHERE movieId = @Id",
            new { movie.Id }, transaction, cancellationToken: ct));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT INTO genres (movieId, name) VALUES (@MovieId, @Name)",
                new { MovieId = movie.Id, Name = genre }, transaction));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE movies
            SET title = @Title, slug = @Slug, yearOfRelease = @YearOfRelease
            WHERE id = @Id;
            """,
            movie,
            transaction,
            cancellationToken: ct
        ));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM genres WHERE movieId = @Id",
            new { Id = id }, transaction, cancellationToken: ct));

        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM movies
            WHERE id = @Id;
            """,
            new { Id = id },
            transaction,
            cancellationToken: ct
        ));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("SELECT COUNT(1) FROM movies WHERE id = @Id", new { Id = id }, cancellationToken: ct)
        );
    }
}