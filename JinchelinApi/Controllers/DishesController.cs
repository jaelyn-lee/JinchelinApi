using JinchelinApi.Data;
using JinchelinApi.DTOs;
using JinchelinApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JinchelinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishesController(AppDbContext db) : ControllerBase
{
    // GET /api/dishes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishResponse>>> GetAll(
        [FromQuery] string?  search,
        [FromQuery] Guid?    categoryId,
        [FromQuery] string   sortBy = "rating")
    {
        var query = db.Dishes
            .Include(d => d.Category)
            .Include(d => d.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Name.ToLower().Contains(search.ToLower()));

        if (categoryId.HasValue)
            query = query.Where(d => d.CategoryId == categoryId);

        var dishes = await query.ToListAsync();

        var result = dishes.Select(ToResponse).AsEnumerable();

        result = sortBy switch
        {
            "name" => result.OrderBy(d => d.Name),
            "date" => result.OrderByDescending(d => d.CreatedAt),
            _      => result.OrderByDescending(d => d.AvgRating)
        };

        return Ok(result);
    }

    // GET /api/dishes/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DishResponse>> GetById(Guid id)
    {
        var dish = await db.Dishes
            .Include(d => d.Category)
            .Include(d => d.Reviews)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dish is null) return NotFound();
        return Ok(ToResponse(dish));
    }

    // GET /api/dishes/ranking - Hall of fame
    [HttpGet("ranking")]
    public async Task<ActionResult<IEnumerable<DishResponse>>> GetRanking()
    {
        var dishes = await db.Dishes
            .Include(d => d.Category)
            .Include(d => d.Reviews)
            .Where(d => d.Reviews.Any())
            .ToListAsync();

        var ranked = dishes
            .Select(ToResponse)
            .OrderByDescending(d => d.AvgRating)
            .ThenByDescending(d => d.ReviewCount);

        return Ok(ranked);
    }

    // POST /api/dishes
    [HttpPost]
    public async Task<ActionResult<DishResponse>> Create([FromBody] CreateDishRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return BadRequest("Dish name is required.");

        var dish = new Dish { Name = req.Name, CategoryId = req.CategoryId };
        db.Dishes.Add(dish);
        await db.SaveChangesAsync();

        var created = await db.Dishes
            .Include(d => d.Category)
            .Include(d => d.Reviews)
            .FirstAsync(d => d.Id == dish.Id);

        return CreatedAtAction(nameof(GetById), new { id = dish.Id }, ToResponse(created));
    }

    // DELETE /api/dishes/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var dish = await db.Dishes.FindAsync(id);
        if (dish is null) return NotFound();

        db.Dishes.Remove(dish);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // ── Helper ────────────────────────────────────────────
    private static DishResponse ToResponse(Dish d) => new(
        d.Id,
        d.Name,
        d.Category?.Name,
        d.Category?.Cuisine,
        d.Reviews.Any() ? Math.Round(d.Reviews.Average(r => r.Rating), 1) : 0,
        d.Reviews.Count,
        d.CreatedAt
    );
}
