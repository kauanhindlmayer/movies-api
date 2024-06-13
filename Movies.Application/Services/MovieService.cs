using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IValidator<Movie> validator) : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(movie, ct);
        return await movieRepository.CreateAsync(movie, ct);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return movieRepository.GetByIdAsync(id, ct);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return movieRepository.GetBySlugAsync(slug, ct);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default)
    {
        return movieRepository.GetAllAsync(ct);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(movie, ct);
        var movieExists = await movieRepository.ExistsByIdAsync(movie.Id, ct);
        if (!movieExists)
        {
            return null;
        }

        await movieRepository.UpdateAsync(movie, ct);
        return movie;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return movieRepository.DeleteAsync(id, ct);
    }
}