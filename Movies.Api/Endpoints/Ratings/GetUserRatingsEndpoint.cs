using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Create,
            async (IRatingService ratingService, HttpContext httpContext, CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, ct);
                var ratingsResponse = ratings.MapToResponse();
                return TypedResults.Ok(ratingsResponse);
            });
        return app;
    }
}