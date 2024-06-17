using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class GetMovieEndpoint
{
    public const string Name = "GetMovie";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Get,
            async (string idOrSlug, IMovieService movieService, HttpContext httpContext, CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, ct)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, ct);

                if (movie is null)
                {
                    return Results.NotFound();
                }

                var movieResponse = movie.MapToResponse();
                return TypedResults.Ok(movieResponse);
            });
        return app;
    }
}