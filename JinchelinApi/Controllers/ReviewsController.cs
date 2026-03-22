using JinchelinApi.Data;
using JinchelinApi.DTOs;
using JinchelinApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JinchelinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(AppDbContext db) : ControllerBase
{
    // GET /api/reviews  — all reviews, newest first
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await db.Reviews
            .Include(r => r.Dish)
                .ThenInclude(d => d!.Category)
            .OrderByDescending(r => r.ReviewedAt)
            .ToListAsync();

        return Ok(reviews.Select(ToResponse));
    }

    // GET /api/reviews/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await db.Reviews
            .Include(r => r.Dish)
                .ThenInclude(d => d!.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review is null) return NotFound();
        return Ok(ToResponse(review));
    }

    // GET /api/reviews/dish/{dishId}  — all reviews for one dish
    [HttpGet("dish/{dishId:guid}")]
    public async Task<IActionResult> GetByDish(Guid dishId)
    {
        var reviews = await db.Reviews
            .Include(r => r.Dish)
                .ThenInclude(d => d!.Category)
            .Where(r => r.DishId == dishId)
            .OrderByDescending(r => r.ReviewedAt)
            .ToListAsync();

        return Ok(reviews.Select(ToResponse));
    }

    // POST /api/reviews
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest req)
    {
        if (req.Rating is < 1 or > 5)
            return BadRequest("Rating must be between 1 and 5.");

        var dishExists = await db.Dishes.AnyAsync(d => d.Id == req.DishId);
        if (!dishExists) return BadRequest("Dish not found.");

        var review = new Review
        {
            DishId     = req.DishId,
            Rating     = req.Rating,
            ReviewText = req.ReviewText,
            PhotoUrl   = req.PhotoUrl,
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync();

        var created = await db.Reviews
            .Include(r => r.Dish)
                .ThenInclude(d => d!.Category)
            .FirstAsync(r => r.Id == review.Id);

        return CreatedAtAction(nameof(GetById), new { id = review.Id }, ToResponse(created));
    }

    // PATCH /api/reviews/{id}/photo  — attach photo URL after upload
    [HttpPatch("{id:guid}/photo")]
    public async Task<IActionResult> UpdatePhoto(Guid id, [FromBody] string photoUrl)
    {
        var review = await db.Reviews.FindAsync(id);
        if (review is null) return NotFound();

        review.PhotoUrl = photoUrl;
        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/reviews/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var review = await db.Reviews.FindAsync(id);
        if (review is null) return NotFound();

        db.Reviews.Remove(review);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ── Helper ────────────────────────────────────────────
    private static ReviewResponse ToResponse(Review r) => new(
        r.Id,
        r.DishId,
        r.Dish?.Name ?? string.Empty,
        r.Dish?.Category?.Name,
        r.Rating,
        r.ReviewText,
        r.PhotoUrl,
        r.ReviewedAt,
        r.CreatedAt
    );
}
