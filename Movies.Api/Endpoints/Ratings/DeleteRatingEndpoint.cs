using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    private const string Name = "DeleteRating";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (
                Guid id, IRatingService ratingService, HttpContext httpContext, CancellationToken ct) =>
            {
                var userId = httpContext.GetUserId();
                var result = await ratingService.DeleteRatingAsync(id, userId!.Value, ct);
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