using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken ct = default);

    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken ct = default);

    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken ct = default);

    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken ct = default);

    Task<bool> UpdateAsync(Movie movie, CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken ct = default);
}