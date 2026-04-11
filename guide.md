````block markdown
# Zero Trust API Blueprint – Development Guide

This guide is incrementally updated with each completed story.
It contains the exact steps to initialize, build, test, and deploy the Enterprise Zero Trust API.

---

## Phase 0: Project Initialization (Complete)

We set up a .NET 10 Web API using **controllers** (not minimal APIs), built‑in OpenAPI, Docker Compose, and a GitHub repository with a project board.

### 1. Create local solution and API project

```shell
mkdir zero-trust-api-blueprint
cd zero-trust-api-blueprint
dotnet new sln -n ZeroTrustAPI
dotnet new webapi --use-controllers -n ZeroTrustAPI.Api -o src/ZeroTrustAPI.Api
dotnet sln add src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj
````

### 2. Add test project

```shell
dotnet new xunit -n ZeroTrustAPI.Tests -o tests/ZeroTrustAPI.Tests
dotnet sln add tests/ZeroTrustAPI.Tests/ZeroTrustAPI.Tests.csproj
dotnet add tests/ZeroTrustAPI.Tests reference src/ZeroTrustAPI.Api
```

### 3. Replace default `WeatherForecastController` with `HealthController`

Delete the default files:

```shell
del src\ZeroTrustAPI.Api\Controllers\WeatherForecastController.cs
del src\ZeroTrustAPI.Api\WeatherForecast.cs
```

Create `src\ZeroTrustAPI.Api\Controllers\HealthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace ZeroTrustAPI.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
```

### 4. `Program.cs` (unchanged from template – uses built‑in OpenAPI)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 5. Add Docker support

**Dockerfile** (in solution root):

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj", "src/ZeroTrustAPI.Api/"]
RUN dotnet restore "src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj"
COPY . .
WORKDIR "/src/src/ZeroTrustAPI.Api"
RUN dotnet build "ZeroTrustAPI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZeroTrustAPI.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZeroTrustAPI.Api.dll"]
```

**docker-compose.yml** (in solution root):

--yaml
version: '3.8'
services:
api:
build: .
ports: - "5000:80" - "5001:443"
environment: - ASPNETCORE_ENVIRONMENT=Development - ASPNETCORE_URLS=https://+:443;http://+:80

````

### 6. Build and test locally

```shell
dotnet restore
dotnet build
dotnet run --project src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj
````

Visit `http://localhost:5xxx/health` → `{"status":"healthy","timestamp":"..."}`.  
Press `Ctrl+C` to stop.

### 7. Test with Docker Compose

```shell
docker-compose up --build
```

Visit `http://localhost:5000/health` → same JSON response.

### 8. Push to GitHub

Create an **empty** repository on GitHub, then:

```shell
git init
git add .
git commit -m "Initial Zero Trust API with controllers, built-in OpenAPI, Docker"
git remote add origin https://github.com/yourusername/zero-trust-api-blueprint.git
git branch -M main
git push -u origin main --force
```

### 9. Create GitHub Project board

- Go to the repository → **Projects** → **New project** → **Board**.
- Name: `Zero Trust Delivery`
- Columns: `Backlog`, `Ready for Sprint`, `In Progress`, `Review`, `Done`.

### 10. Create the first user story (Issue)

**Title:** `Story 1.1: MFA Enforcement (Section 1)`

**Body** (Gherkin acceptance criteria):

```gherkin
Scenario: Successful authentication with MFA
  Given a user with valid credentials and a registered MFA device
  When the user submits username/password + valid TOTP code
  Then the API returns a short-lived JWT access token

Scenario: Missing MFA token
  Given a user with valid credentials but no MFA code
  When the user submits only username/password
  Then the API returns 401 Unauthorized

Scenario: Invalid MFA token
  Given a user with valid credentials
  When the user submits an incorrect TOTP code
  Then the API returns 401 Unauthorized
```

Add the issue to the `Zero Trust Delivery` board (right sidebar → Projects).

---

## Next steps (to be added incrementally)

Each new story will add:

- A feature branch from `main`
- TDD tests written first (failing)
- Implementation of the Zero Trust control (e.g., JWT + MFA, RBAC, mTLS, webhook security)
- Pull request with two reviews
- Merging and moving the issue to `Done`

This guide will be updated with every completed story.

---

**Current status:** Initialization complete. Ready for Story 1.1.

```

```
