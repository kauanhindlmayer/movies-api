using Dapper;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS movies (
                                          id UUID PRIMARY KEY,
                                          title TEXT NOT NULL,
                                          slug TEXT NOT NULL,
                                          yearofrelease INTEGER NOT NULL
                                      );
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS idx_movies_slug
                                      ON movies (slug);
                                      """);
        
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS genres (
                                          movieId UUID REFERENCES movies (id),
                                          name TEXT NOT NULL
                                      );
                                      """);
    }
}