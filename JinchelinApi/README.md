# JinchelinApi — Setup Guide

## Prerequisites
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- Supabase project already set up (schema.sql executed)

---

## 1. Create the project

```bash
mkdir JinchelinApi && cd JinchelinApi
dotnet new webapi --no-https -n JinchelinApi
# Then replace the generated files with the provided ones
```

## 2. Install packages

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Supabase --version 1.1.1
dotnet add package dotenv.net --version 3.1.3
```

## 3. Set environment variables

```bash
cp .env.example .env
# Open .env and fill in your Supabase values:
#   CONNECTION_STRING  → Supabase Dashboard → Project Settings → Database → .NET connection string
#   SUPABASE_URL       → Project Settings → API → Project URL
#   SUPABASE_SERVICE_ROLE_KEY → Project Settings → API → service_role key
```

## 4. Run locally

```bash
dotnet run
# Swagger UI opens at: http://localhost:5000/swagger
```

---

## API Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET    | /api/dishes | All dishes (search, filter, sort) |
| GET    | /api/dishes/ranking | Hall of Fame ranking |
| GET    | /api/dishes/{id} | Single dish |
| POST   | /api/dishes | Create dish |
| DELETE | /api/dishes/{id} | Delete dish |
| GET    | /api/reviews | All reviews |
| GET    | /api/reviews/{id} | Single review |
| GET    | /api/reviews/dish/{dishId} | Reviews for a dish |
| POST   | /api/reviews | Create review |
| PATCH  | /api/reviews/{id}/photo | Attach photo URL |
| DELETE | /api/reviews/{id} | Delete review |
| POST   | /api/upload | Upload photo → returns URL |
| GET    | /api/categories | All categories |

---

## Folder Structure

```
JinchelinApi/
├── Controllers/
│   ├── DishesController.cs
│   ├── ReviewsController.cs
│   ├── UploadController.cs
│   └── CategoriesController.cs
├── Data/
│   └── AppDbContext.cs
├── DTOs/
│   └── DTOs.cs
├── Models/
│   └── Models.cs
├── Program.cs
├── JinchelinApi.csproj
├── .env.example
└── .gitignore
```

---

## Flow: Adding a new review with photo

```
1. POST /api/dishes        → create dish (if new)
2. POST /api/upload        → upload photo → get { url }
3. POST /api/reviews       → create review with photoUrl from step 2
```
