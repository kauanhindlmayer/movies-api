using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO movies (id, title, slug, yearofrelease)
            VALUES (@Id, @Title, @Slug, @YearOfRelease);
            """,
            movie,
            transaction
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
                    transaction
                ));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                SELECT id, title, slug, yearofrelease
                FROM movies
                WHERE id = @Id;
                """,
                new { Id = id }
            )
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
                new { MovieId = id }
            )
        );

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(
                """
                SELECT id, title, slug, yearofrelease
                FROM movies
                WHERE slug = @Slug;
                """,
                new { Slug = slug }
            )
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
                new { MovieId = movie.Id }
            )
        );

        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync(
            """
            SELECT m.*, STRING_AGG(g.name, ', ') AS genres
            FROM movies m
            LEFT JOIN genres g ON m.id = g.movieId
            GROUP BY m.id;
            """
        );

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(", "))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM genres WHERE movieId = @Id",
            new { movie.Id }, transaction));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("INSERT INTO genres (movieId, name) VALUES (@MovieId, @Name)",
                new { MovieId = movie.Id, Name = genre }, transaction));
        }
        
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE movies
            SET title = @Title, slug = @Slug, yearOfRelease = @YearOfRelease
            WHERE id = @Id;
            """,
            movie,
            transaction
        ));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM genres WHERE movieId = @Id",
            new { Id = id }, transaction));
        
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM movies
            WHERE id = @Id;
            """,
            new { Id = id },
            transaction
        ));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("SELECT COUNT(1) FROM movies WHERE id = @Id", new { Id = id })
        );
    }
}