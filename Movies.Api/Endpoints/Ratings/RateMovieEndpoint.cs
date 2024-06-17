using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Ratings;

public static class RateMovieEndpoint
{
    private const string Name = "RateMovie";

    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Rate, async (
                Guid id, RateMovieRequest request, IRatingService ratingService, HttpContext httpContext,
                CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var result = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, ct);
                return result ? TypedResults.Ok() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(Tags.Ratings)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }
}