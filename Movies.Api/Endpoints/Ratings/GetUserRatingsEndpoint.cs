using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    private const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                IRatingService ratingService, HttpContext httpContext, CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, ct);
                var ratingsResponse = ratings.MapToResponse();
                return TypedResults.Ok(ratingsResponse);
            })
            .WithName(Name)
            .WithTags(Tags.Ratings)
            .Produces<IEnumerable<MovieRatingResponse>>()
            .RequireAuthorization();

        return app;
    }
}