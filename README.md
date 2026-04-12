# Zero Trust API Blueprint

[![.NET](https://github.com/your-username/zero-trust-api-blueprint/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/your-username/zero-trust-api-blueprint/actions/workflows/ci-cd.yml)
[![Coverage](https://img.shields.io/badge/coverage-≥85%25-brightgreen)](https://github.com/your-username/zero-trust-api-blueprint)
[![License](https://img.shields.io/badge/license-Proprietary-red)](LICENSE)

**Enterprise Zero Trust API implementation** – .NET 11 preview, layered architecture (Controllers → Services → Repositories → Mappers), TDD with >85% coverage, Docker Compose.

This repository follows **Zero Trust principles**: no implicit trust, continuous verification, least privilege, and defense in depth.

---

## 🚀 Quick Start

### Prerequisites

- [.NET 11 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/11.0) (preview)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional)
- Git

### Run with Docker (recommended)

```bash
git clone https://github.com/yourusername/zero-trust-api-blueprint.git
cd zero-trust-api-blueprint
docker-compose up --build
```

API available at `http://localhost:5000`

Test the health endpoint:

```bash
curl http://localhost:5000/health
```

Expected response:

```json
{ "status": "healthy", "timestamp": "2025-04-11T..." }
```

### Run locally with .NET CLI

```bash
dotnet restore
dotnet run --project src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj
```

---

## 📡 API Endpoints

| Method | Endpoint          | Description         | Request Body                                     | Response                                                    |
| ------ | ----------------- | ------------------- | ------------------------------------------------ | ----------------------------------------------------------- |
| POST   | `/api/auth/login` | Authenticate a user | `{ "username": "string", "password": "string" }` | `200 OK` with `AuthResult` or `401 Unauthorized`            |
| GET    | `/health`         | Health check        | –                                                | `200 OK` with `{ "status": "healthy", "timestamp": "..." }` |

**Example login request:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "john", "password": "secret"}'
```

---

## 🧪 Testing

Run all tests (unit + integration) with coverage:

```bash
./run-coverage.sh
```

This will:

- Execute all tests (xUnit)
- Generate an HTML coverage report in `ZeroTrustAPI.Tests/CoverageReport/index.html`
- Enforce minimum 85% line coverage (excluding generated code)

Run tests without coverage:

```bash
cd tests/ZeroTrustAPI.Tests
dotnet test
```

All tests follow the **AAA (Arrange‑Act‑Assert)** pattern. Unit tests use mocks (Moq); integration tests use `WebApplicationFactory` with an in‑memory database.

---

## 🏗️ Architecture

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed layer descriptions (Controllers → Services → Repositories → Mappers), dependency injection, and the TDD workflow.

**Layered structure:**

```bash
Controllers → Services → Repositories → Mappers → Entities/DTOs
```

- **Controllers** – handle HTTP, delegate to service interfaces.
- **Services** – business logic, orchestrate repositories.
- **Repositories** – abstract data access (EF Core with conditional InMemory for tests).
- **Mappers** – convert between entities and DTOs.
- **DTOs** – API contracts (request/response).
- **Entities** – database models.

All dependencies are injected via constructors (no `new` inside classes). Environment variable `TESTING=true` switches to InMemory database for integration tests.

---

## 🐳 Docker Compose

`docker-compose.yml` launches the API on port `5000` (host) → `8080` (container).

```yaml
services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

---

## 📁 Project Structure

```bash
zero-trust-api-blueprint/
├── src/
│   └── ZeroTrustAPI.Api/
│       ├── Controllers/
│       ├── Services/
│       ├── Repositories/
│       ├── Mappers/
│       ├── DTOs/
│       ├── Entities/
│       ├── Data/
│       ├── Security/
│       └── Program.cs
├── tests/
│   └── ZeroTrustAPI.Tests/
│       ├── Unit/
│       └── Integration/
├── docs/
│   └── blueprint.md
├── Dockerfile
├── docker-compose.yml
├── coverlet.runsettings
├── run-coverage.sh
└── README.md
```

---

## 🛣️ Roadmap

Current status: ✅ **Login endpoint implemented** with layered architecture, DI, repository, mapper, and full test coverage (>85%).  
Next: **Story 1.1 – MFA Enforcement (JWT + TOTP)**.

---

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for TDD requirements, AAA pattern, signed commits, two‑person review, and security policies.

---

## 📄 License

Proprietary – internal enterprise use.

**Built with .NET 11 preview, layered architecture, and Zero Trust principles.**
