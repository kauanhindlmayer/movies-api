using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Ratings;

public static class RateMovieEndpoint
{
    public const string Name = "RateMovie";

    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Create,
                async (Guid id, RateMovieRequest request, IRatingService ratingService, HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var userId = httpContext.GetUserId();
                    var result = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, ct);
                    return result ? TypedResults.Ok() : Results.NotFound();
                })
            .WithName(Name)
            .RequireAuthorization();

        return app;
    }
}