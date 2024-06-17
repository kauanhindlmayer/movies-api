// using Asp.Versioning;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Movies.Api.Auth;
// using Movies.Api.Mappings;
// using Movies.Application.Services;
// using Movies.Contracts.Requests;
// using Movies.Contracts.Responses;
//
// namespace Movies.Api.Controllers;
//
// [ApiController]
// [ApiVersion(1.0)]
// public class RatingsController(IRatingService ratingService) : ControllerBase
// {
//     [Authorize]
//     [HttpPut(ApiEndpoints.Movies.Rate)]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request,
//         CancellationToken ct)
//     {
//         var userId = HttpContext.GetUserId();
//         var result = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, ct);
//         return result ? Ok() : NotFound();
//     }
//     
//     [Authorize]
//     [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken ct)
//     {
//         var userId = HttpContext.GetUserId();
//         var result = await ratingService.DeleteRatingAsync(id, userId!.Value, ct);
//         return result ? Ok() : NotFound();
//     }
//     
//     [Authorize]
//     [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
//     [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]
//     public async Task<IActionResult> GetUserRatings(CancellationToken ct)
//     {
//         var userId = HttpContext.GetUserId();
//         var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, ct);
//         var ratingsResponse = ratings.MapToResponse();
//         return Ok(ratingsResponse);
//     }
// }