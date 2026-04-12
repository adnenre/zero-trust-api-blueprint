# Contributing to Zero Trust API Blueprint

Thank you for your interest! Please follow these guidelines to ensure code quality, test coverage, and architectural consistency.

---

## Development Workflow

### 1. Branch Naming

- `feature/short-description` – for new features
- `fix/short-description` – for bug fixes
- `docs/short-description` – for documentation changes

### 2. TDD (Test‑Driven Development)

**Write the test first.** Every new feature or bug fix must start with a failing test. Use the sequence:

```
Test → DTO → DIP → DI+SRP → Fake → Service → Repository → Entity → Mapper
```

### 3. AAA Pattern (Arrange‑Act‑Assert)

All tests must clearly separate:

- **Arrange** – set up dependencies (mocks), input data, and expected outcomes.
- **Act** – invoke the method under test.
- **Assert** – verify the result and interactions.

Example (unit test):

```csharp
[Fact]
public async Task Login_ValidCredentials_ReturnsOk()
{
    // Arrange
    var mockService = new Mock<IAuthService>();
    mockService.Setup(s => s.AuthenticateAsync("user", "pass"))
               .ReturnsAsync(new AuthResult(true, new UserDto(1, "user"), "OK"));
    var controller = new AuthController(mockService.Object);

    // Act
    var result = await controller.Login(new LoginRequest("user", "pass"));

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.True(((AuthResult)okResult.Value).Success);
}
```

### 4. Code Coverage

- **Minimum coverage: 85%** (line coverage for your handwritten code).
- Excluded files are defined in `coverlet.runsettings` (generated code, third‑party libraries).
- Run coverage locally before pushing:

```bash
./run-coverage.sh
```

- CI pipeline will reject any PR with coverage below 85%.

### 5. Layered Architecture

Respect the dependency direction:

```
Controllers → Services → Repositories → Mappers → Entities/DTOs
```

- **Controllers** must **not** reference repositories directly.
- **Services** must **not** reference `HttpContext` or other web‑specific types.
- **Repositories** must return entities, never DTOs.
- **Mappers** are stateless (static methods or singleton).

### 6. Dependency Injection

- All dependencies must be passed via constructor injection.
- Do **not** use `new` to instantiate services, repositories, or mappers inside classes.
- Register all interfaces in `Program.cs` with the appropriate lifetime (`AddScoped` by default).

Example registration:

```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IHealthService, HealthService>();
```

### 7. Testing Layers

| Test Type   | Folder               | Dependencies               | Tools                                                             |
| ----------- | -------------------- | -------------------------- | ----------------------------------------------------------------- |
| Unit        | `tests/Unit/`        | Mocks (Moq)                | `xUnit`, `Moq`                                                    |
| Integration | `tests/Integration/` | Real pipeline, InMemory DB | `WebApplicationFactory`, `Microsoft.EntityFrameworkCore.InMemory` |

**Integration test setup:**

```csharp
Environment.SetEnvironmentVariable("TESTING", "true");
var factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder => { ... });
var client = factory.CreateClient();
```

### 8. Pull Request Process

1. Create a branch from `main`.
2. Write tests (red).
3. Implement code (green).
4. Refactor while keeping tests green.
5. Ensure coverage ≥85% (`./run-coverage.sh`).
6. Push and open a Pull Request.
7. Wait for CI checks to pass (build, test, coverage, security scan, Docker build).
8. Request a review (if applicable) or merge yourself after CI is green.

### 9. Commit Messages

Use conventional commits:

```
feat: add login endpoint with layered architecture
fix: register IHealthService in Program.cs
test: increase coverage for AuthService
docs: update README with API endpoints
```

### 10. Security Policies

- **No secrets in code** – use environment variables or GitHub Secrets.
- **Signed commits** are recommended (but not required for internal repos).
- **Two‑person review** for sensitive changes (e.g., authentication, authorization).

---

## Running Checks Locally

```bash
# Restore, build, and test
dotnet restore
dotnet build
dotnet test

# Full coverage report
./run-coverage.sh

# Build Docker image
docker build -t zero-trust-api .

# Run container
docker run -d -p 5000:8080 zero-trust-api
```

---

## Code of Conduct

Be respectful, constructive, and collaborative. Focus on the code, not the person. Zero Trust also applies to interactions – verify assumptions and assume good intent.

---

## Questions?

Open an issue or contact the maintainer. Thank you for contributing!
