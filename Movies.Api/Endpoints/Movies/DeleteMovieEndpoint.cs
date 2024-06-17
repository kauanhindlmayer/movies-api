using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Delete, async (
                Guid id, IMovieService movieService, IOutputCacheStore outputCacheStore,
                CancellationToken ct) =>
            {
                var deleted = await movieService.DeleteAsync(id, ct);
                if (!deleted)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync("movies", ct);
                return Results.NoContent();
            })
            .WithName(Name)
            .RequireAuthorization(AuthConstants.AdminUserPolicyName);

        return app;
    }
}