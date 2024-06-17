using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    private const string Name = "CreateMovie";

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
            })
            .WithName(Name)
            .WithTags(Tags.Movies)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedUserPolicyName);

        return app;
    }
}