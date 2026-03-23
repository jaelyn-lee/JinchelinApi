using dotenv.net;
using JinchelinApi.Data;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────
var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING not set.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

// ── Supabase Storage client ───────────────────────────────
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
    ?? throw new InvalidOperationException("SUPABASE_URL not set.");
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY")
    ?? throw new InvalidOperationException("SUPABASE_SERVICE_ROLE_KEY not set.");

builder.Services.AddSingleton(_ => new Supabase.Client(
    supabaseUrl,
    supabaseKey,
    new Supabase.SupabaseOptions { AutoConnectRealtime = false }
));

// ── CORS (allow React dev server + Capacitor app) ─────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config => {
    config.Title = "Jinchelin API";
    config.Version = "v1";
});

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
