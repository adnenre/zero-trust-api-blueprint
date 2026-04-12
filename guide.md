````block markdown
# Zero Trust API Blueprint – Development Guide

This guide is incrementally updated with each completed story.
It contains the exact steps to initialize, build, test, and deploy the Enterprise Zero Trust API.

---

## Phase 0: Project Initialization (Complete)

We set up a **.NET 11 preview** Web API using **controllers** (not minimal APIs), built‑in OpenAPI, Docker Compose, and a GitHub repository with a project board.

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

### 5. Add Docker support (corrected for .NET 11 preview)

**Dockerfile** (in solution root) – using .NET 11 preview images:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /app
COPY . .
RUN dotnet restore "src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj"
RUN dotnet publish "src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ZeroTrustAPI.Api.dll"]
```

**docker-compose.yml** (in solution root) – maps host port 5000 to container port 8080:

```yaml
services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

### 6. Build and test locally

```shell
dotnet restore
dotnet build
dotnet run --project src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj
```

Visit `http://localhost:5xxx/health` → `{"status":"healthy","timestamp":"..."}`.  
Press `Ctrl+C` to stop.

### 7. Test with Docker Compose

```shell
docker-compose up --build
```

Visit `http://localhost:5000/health` → same JSON response.

### 8. Add health endpoint test (TDD baseline)

Create `tests/ZeroTrustAPI.Tests/Health/HealthControllerTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ZeroTrustAPI.Api;
using Xunit;

namespace ZeroTrustAPI.Tests.Health;

public record HealthResponse(string status, DateTime timestamp);

public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Ok_With_Status_Healthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(result);
        Assert.Equal("healthy", result?.status);
        Assert.True(result?.timestamp != default);
    }
}
```

Run the test:

```shell
dotnet test
```

It should pass.

### 9. Push to GitHub

Create an **empty** repository on GitHub, then:

```shell
git init
git add .
git commit -m "Initial Zero Trust API with controllers, built-in OpenAPI, Docker"
git remote add origin https://github.com/yourusername/zero-trust-api-blueprint.git
git branch -M main
git push -u origin main --force
```

### 10. Create GitHub Project board

- Go to the repository → **Projects** → **New project** → **Board**.
- Name: `Zero Trust Delivery`
- Columns: `Backlog`, `Ready for Sprint`, `In Progress`, `Review`, `Done`.

### 11. Create the first user story (Issue)

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

### 12. CI/CD pipeline

Create `.github/workflows/ci.yml` with the following jobs: restore, build, test (with coverage ≥85%), security scan, docker build, and push to GHCR (only on `main`). The pipeline uses .NET 11 preview SDK and pushes the image to `ghcr.io/yourusername/zero-trust-api-blueprint:latest`.

---

## Next steps (to be added incrementally)

Each new story will add:

- A feature branch from `main`
- TDD tests written first (failing)
- Implementation of the Zero Trust control (e.g., JWT + MFA, RBAC, webhook security)
- Pull request with two reviews
- Merging and moving the issue to `Done`

This guide will be updated with every completed story.

---

**Current status:** Initialization complete. Health endpoint working, tests passing, Docker image pushed to GHCR. Ready for Story 1.1 (MFA + JWT).
