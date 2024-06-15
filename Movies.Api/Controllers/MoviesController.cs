using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
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
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator,
        CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, ct)
            : await movieService.GetBySlugAsync(idOrSlug, userId, ct);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToResponse();
        return Ok(movieResponse);
    }
    
    [ApiVersion(2.0)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> GetV2([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator,
        CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, ct)
            : await movieService.GetBySlugAsync(idOrSlug, userId, ct);

        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToResponse();
        
        movieResponse.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id })!,
            Rel = "self",
            Type = "GET"
        });
        
        return Ok(movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions().WithUserId(userId);
        var movies = await movieService.GetAllAsync(options, ct);
        var moviesCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, ct);
        var moviesResponse = movies.MapToResponse(options.Page, options.PageSize, moviesCount);
        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        var updatedMovie = await movieService.UpdateAsync(movie, userId, ct);
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