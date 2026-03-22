using JinchelinApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JinchelinApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Dish>     Dishes     => Set<Dish>();
    public DbSet<Review>   Reviews    => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map to snake_case table names (matches Supabase schema)
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Dish>().ToTable("dishes");
        modelBuilder.Entity<Review>().ToTable("reviews");

        // Rating: store as numeric(2,1)
        modelBuilder.Entity<Review>()
            .Property(r => r.Rating)
            .HasColumnType("numeric(2,1)");

        // ReviewedAt: map DateOnly to Postgres date
        modelBuilder.Entity<Review>()
            .Property(r => r.ReviewedAt)
            .HasColumnType("date");

        // Dish → Category: set null on category delete
        modelBuilder.Entity<Dish>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Dishes)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Review → Dish: cascade delete
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Dish)
            .WithMany(d => d.Reviews)
            .HasForeignKey(r => r.DishId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
