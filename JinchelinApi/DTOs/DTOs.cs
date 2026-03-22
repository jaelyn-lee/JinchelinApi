namespace JinchelinApi.DTOs; //TS syntax: export interface DTOs {

// ── Dishes ────────────────────────────────────────────────

public record CreateDishRequest(
    string Name,
    Guid?  CategoryId
);

public record DishResponse(
    Guid     Id,
    string   Name,
    string?  Category,
    string?  Cuisine,
    decimal  AvgRating,
    int      ReviewCount,
    DateTime CreatedAt
);

// ── Reviews ───────────────────────────────────────────────

public record CreateReviewRequest(
    Guid    DishId,
    decimal Rating,        // 1.0 – 5.0
    string? ReviewText,
    string? PhotoUrl       // set after upload; or null if no photo yet
);

public record ReviewResponse(
    Guid      Id,
    Guid      DishId,
    string    DishName,
    string?   Category,
    decimal   Rating,
    string?   ReviewText,
    string?   PhotoUrl,
    DateOnly  ReviewedAt,
    DateTime  CreatedAt
);

// ── Categories ────────────────────────────────────────────

public record CategoryResponse(
    Guid   Id,
    string Name,
    string Cuisine
);

// ── Upload ────────────────────────────────────────────────

public record UploadResponse(string Url); //TS syntax: export interface UploadResponse { url: string; }
