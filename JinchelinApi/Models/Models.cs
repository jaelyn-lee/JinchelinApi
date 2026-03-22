using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JinchelinApi.Models; // In TS, Model is like interface, type, DTO, or class with properties only. No methods or logic. Just data structure.
// Model (entity) => Data that is connected to the DB.
// DTO (data transfer object) => Data that is sent to/from client. For API requests and responses.

// ── Category ─────────────────────────────────────────────
[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("name")]
    [Required]
    public string Name { get; set; } = string.Empty;      // e.g. "Italian"

    [Column("cuisine")]
    [Required]
    public string Cuisine { get; set; } = string.Empty;   // e.g. "Western"

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Dish> Dishes { get; set; } = [];
}

// ── Dish ──────────────────────────────────────────────────
[Table("dishes")]
public class Dish
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("name")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public ICollection<Review> Reviews { get; set; } = [];
}

// ── Review ────────────────────────────────────────────────
[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("dish_id")]
    [Required]
    public Guid DishId { get; set; }

    [Column("rating")]
    [Range(1.0, 5.0)]
    public decimal Rating { get; set; }

    [Column("review_text")]
    public string? ReviewText { get; set; }

    [Column("photo_url")]
    public string? PhotoUrl { get; set; }

    [Column("reviewed_at")]
    public DateOnly ReviewedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("DishId")]
    public Dish? Dish { get; set; }
}
