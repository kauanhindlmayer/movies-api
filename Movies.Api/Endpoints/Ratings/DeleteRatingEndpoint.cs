using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public const string Name = "DeleteRating";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Create, async (Guid id, IRatingService ratingService,
            HttpContext httpContext, CancellationToken ct) =>
        {
            var userId = httpContext.GetUserId();
            var result = await ratingService.DeleteRatingAsync(id, userId!.Value, ct);
            return result ? TypedResults.Ok() : Results.NotFound();
        });
        return app;
    }
}