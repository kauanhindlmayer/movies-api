﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingsController(IRatingService ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request,
        CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, ct);
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var result = await ratingService.DeleteRatingAsync(id, userId!.Value, ct);
        return result ? Ok() : NotFound();
    }
}