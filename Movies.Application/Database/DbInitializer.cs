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
                                          yearOfRelease INT NOT NULL
                                      );
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX IF NOT EXISTS idx_movies_slug
                                      ON movies (slug)
                                      USING BTREE(slug);
                                      """);
    }
}