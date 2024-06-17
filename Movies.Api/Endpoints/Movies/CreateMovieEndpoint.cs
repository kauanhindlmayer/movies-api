using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create, async (CreateMovieRequest request, IMovieService movieService,
            IOutputCacheStore outputCacheStore, CancellationToken ct) =>
        {
            var movie = request.MapToMovie();
            await movieService.CreateAsync(movie, ct);
            await outputCacheStore.EvictByTagAsync("movies", ct);
            var movieResponse = movie.MapToResponse();
            return TypedResults.CreatedAtRoute(movieResponse, GetMovieEndpoint.Name,
                new { idOrSlug = movieResponse.Id });
        });
        return app;
    }
}