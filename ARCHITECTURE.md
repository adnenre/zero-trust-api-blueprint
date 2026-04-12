# Architecture – Zero Trust API Blueprint

ZeroTrustAPI follows a **layered architecture** (also known as N‑tier) with strict separation of concerns and dependency injection. The design aligns with Zero Trust principles: every request must be verified, least privilege, and assume breach.

---

## Layer Overview

```bash
[ Controller ]  →  [ Service ]  →  [ Repository ]  →  [ Database ]
      ↑                 ↑                ↑
   (DTOs)           (Mappers)        (Entities)
```

### 1. Presentation Layer (Controllers)

- **Folder:** `Controllers/`
- **Responsibility:** Handle HTTP requests/responses, validate input, return appropriate HTTP status codes.
- **Rules:** No business logic, no direct database access. Depend only on service interfaces (via constructor injection).
- **Example:** `AuthController.Login(LoginRequest)`

### 2. Business Logic Layer (Services)

- **Folder:** `Services/Implementations/` + `Services/Interfaces/`
- **Responsibility:** Implement use cases, enforce business rules, orchestrate repositories and external clients.
- **Rules:** Depend on abstractions (interfaces) – not concrete repositories. Use mappers to convert entities to DTOs. Never reference `HttpContext` or web‑specific types.
- **Example:** `AuthService.AuthenticateAsync(string, string)`

### 3. Data Access Layer (Repositories)

- **Folder:** `Repositories/Implementations/` + `Repositories/Interfaces/`
- **Responsibility:** Abstract database operations (CRUD). Translate between entities and the ORM (Entity Framework Core).
- **Rules:** Return entities, never DTOs. Hide the underlying database provider (SQL Server, InMemory, etc.).
- **Example:** `UserRepository.GetByUsernameAsync(string)`

### 4. Entity Layer (Domain Models)

- **Folder:** `Entities/`
- **Responsibility:** Represent database tables as plain C# objects (POCOs). Contain identity, fields, and relationships.
- **Rules:** No business logic – only data.

### 5. Data Transfer Objects (DTOs)

- **Folder:** `DTOs/`
- **Responsibility:** Define the shape of data sent to/from the client. Decouple internal entities from external contracts.
- **Rules:** No logic, only public properties/constructors. Example: `LoginRequest`, `AuthResult`, `UserDto`.

### 6. Mappers

- **Folder:** `Mappers/`
- **Responsibility:** Convert between entities and DTOs. Centralise mapping logic to avoid repetition.
- **Rules:** Stateless, thread‑safe (static methods or singleton registration). Example: `UserMapper.ToDto(User)`.

### 7. Cross‑cutting Concerns

- **Security:** `Security/` – password hashing (BCrypt), token generation, etc.
- **Data Context:** `Data/` – `AppDbContext` (EF Core) and configuration.
- **Configuration:** `Program.cs` – registers all dependencies and sets up the pipeline.

---

## Dependency Injection

All dependencies are registered in `Program.cs` using `AddScoped<>()` (or `AddSingleton` where appropriate). No `new` operators are used inside classes.

```csharp
// Program.cs
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IHealthService, HealthService>();
```

### Conditional Database Provider

The environment variable `TESTING` switches between real SQL Server and InMemory database:

```csharp
if (Environment.GetEnvironmentVariable("TESTING") == "true")
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
```

This allows integration tests to run without a real database instance.

---

## Testing Strategy

### Unit Tests (`tests/Unit/`)

- Test one class in isolation using mocks (Moq).
- Follow **AAA** pattern (Arrange, Act, Assert).
- Mock all dependencies (e.g., `Mock<IUserRepository>` for `AuthService`).
- Example:

```csharp
[Fact]
public async Task AuthenticateAsync_ValidUser_ReturnsSuccess()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByUsernameAsync("john"))
            .ReturnsAsync(new User { ... });
    // Act
    var result = await service.AuthenticateAsync("john", "pass");
    // Assert
    Assert.True(result.Success);
}
```

### Integration Tests (`tests/Integration/`)

- Test the full HTTP pipeline using `WebApplicationFactory<Program>`.
- Use **InMemory database** (EF Core) to avoid real databases.
- Set environment variable `TESTING=true` before building the factory.
- Example:

```csharp
Environment.SetEnvironmentVariable("TESTING", "true");
var factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder => { ... });
var client = factory.CreateClient();
var response = await client.PostAsJsonAsync("/api/auth/login", request);
```

### Code Coverage

- **Minimum required: 85%** line coverage for handwritten code.
- Excluded files (generated code, third‑party libraries) are defined in `coverlet.runsettings`.
- Run locally: `./run-coverage.sh` (from solution root).
- CI pipeline fails if coverage is below threshold.

---

## TDD Workflow (Test → DTO → DIP → DI+SRP → Fake → Service → Repository → Entity → Mapper)

Every new feature follows this exact sequence:

1. **Test** – Write a failing test (unit or integration).
2. **DTO** – Create request/response data contracts.
3. **DIP** – Define an interface for the service.
4. **DI+SRP** – Build the controller with constructor injection.
5. **Fake** – Implement minimal service to pass the test (optional for unit tests with mocks).
6. **Service** – Write real business logic, inject repositories.
7. **Repository** – Create repository interface and implementation.
8. **Entity** – Define the database model.
9. **Mapper** – Map entity to DTO and vice‑versa.

This ensures that the design is driven by requirements (tests) and remains decoupled.

---

## Security Considerations (Zero Trust)

- **No hardcoded secrets** – use environment variables or secret managers.
- **Password hashing** – BCrypt (adaptive, salt‑included).
- **Input validation** – performed at controller level (DTO validation) and service level (business rules).
- **Logging** – structured logs with correlation IDs (planned).
- **JWT** – planned for next story.

---

## References

- [Zero Trust API Blueprint (29 sections)](docs/blueprint.md)
- [Contributing Guidelines](CONTRIBUTING.md)
- [Development Guide](guide.md)
