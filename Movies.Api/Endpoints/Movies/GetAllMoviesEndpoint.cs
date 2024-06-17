using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetAllMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll, async (
                [AsParameters] GetAllMoviesRequest request, IMovieService movieService, HttpContext httpContext,
                CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var options = request.MapToOptions().WithUserId(userId);
                var movies = await movieService.GetAllAsync(options, ct);
                var moviesCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, ct);
                var moviesResponse = movies.MapToResponse(options.Page, options.PageSize, moviesCount);
                return TypedResults.Ok(moviesResponse);
            })
            .WithName(Name);
        return app;
    }
}