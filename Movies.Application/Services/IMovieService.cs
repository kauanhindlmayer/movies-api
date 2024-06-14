using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken ct = default);

    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken ct = default);

    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken ct = default);

    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken ct = default);

    Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}