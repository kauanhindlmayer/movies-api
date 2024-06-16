using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
    
    [Get(ApiEndpoints.Movies.GetAll)]
    Task<IEnumerable<MovieResponse>> GetAllMoviesAsync(GetAllMoviesRequest request);
    
    [Post(ApiEndpoints.Movies.Create)]
    Task<MovieResponse> CreateMovieAsync([Body] CreateMovieRequest request);
    
    [Put(ApiEndpoints.Movies.Update)]
    Task<MovieResponse> UpdateMovieAsync(Guid id, [Body] UpdateMovieRequest request);
    
    [Delete(ApiEndpoints.Movies.Delete)]
    Task DeleteMovieAsync(Guid id);
}