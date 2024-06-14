using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken ct)
    {
        var movie = request.MapToMovie();
        await movieService.CreateAsync(movie, ct);
        var movieResponse = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken ct)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, ct)
            : await movieService.GetBySlugAsync(idOrSlug, ct);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToResponse();
        return Ok(movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var movies = await movieService.GetAllAsync(ct);
        var moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken ct)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await movieService.UpdateAsync(movie, ct);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await movieService.DeleteAsync(id, ct);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}