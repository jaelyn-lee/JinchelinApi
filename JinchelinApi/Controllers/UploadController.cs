using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace JinchelinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController(Client supabase) : ControllerBase
{
    private const string Bucket = "dish-photos";

    // POST /api/upload
    // Body: multipart/form-data  field: "file"
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file provided.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/heic" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Only JPEG, PNG, WebP, and HEIC images are allowed.");

        // Unique filename: timestamp + original extension
        var ext      = Path.GetExtension(file.FileName);
        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{ext}";

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var bytes = ms.ToArray();

        await supabase.Storage
            .From(Bucket)
            .Upload(bytes, fileName, new Supabase.Storage.FileOptions
            {
                ContentType = file.ContentType,
                Upsert = false
            });

        var publicUrl = supabase.Storage
            .From(Bucket)
            .GetPublicUrl(fileName);

        return Ok(new { url = publicUrl });
    }
}
