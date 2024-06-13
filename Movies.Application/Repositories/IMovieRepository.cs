using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken ct = default);
    
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken ct = default);
    
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default);
    
    Task<bool> UpdateAsync(Movie movie, CancellationToken ct = default);
    
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
}