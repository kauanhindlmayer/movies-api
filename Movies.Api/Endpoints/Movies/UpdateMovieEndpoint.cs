using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update, async (
            Guid id, UpdateMovieRequest request, IMovieService movieService,
            IOutputCacheStore outputCacheStore, HttpContext httpContext, CancellationToken ct) =>
        {
            var userId = httpContext.GetUserId();
            var movie = request.MapToMovie(id);
            var updatedMovie = await movieService.UpdateAsync(movie, userId, ct);
            if (updatedMovie is null)
            {
                return Results.NotFound();
            }

            await outputCacheStore.EvictByTagAsync("movies", ct);
            var response = updatedMovie.MapToResponse();
            return TypedResults.Ok(response);
        });
        return app;
    }
}