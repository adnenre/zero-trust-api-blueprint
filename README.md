# Zero Trust API Blueprint

**Enterprise Zero Trust API implementation** – .NET 11 preview, controllers only, Docker Compose, TDD-ready.

This repository contains the reference implementation of the [Enterprise Zero Trust API Blueprint – Complete Edition (29 sections)](docs/blueprint.md). It follows **Zero Trust principles**: no implicit trust, continuous verification, least privilege, and defense in depth.

---

## 🚀 Quick Start

### Prerequisites

- [.NET 11 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/11.0) (preview)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for containerized run)
- Git

### Run with Docker (recommended)

```shell
git clone https://github.com/yourusername/zero-trust-api-blueprint.git
cd zero-trust-api-blueprint
docker-compose up --build
```

The API will be available at `http://localhost:5000`

Test the health endpoint:

```shell
curl http://localhost:5000/health
```

Expected response:

```json
{ "status": "healthy", "timestamp": "2025-04-11T..." }
```

### Run locally with .NET CLI

```shell
dotnet restore
dotnet run --project src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj
```

Then open `http://localhost:5xxx/health` (port shown in console).

---

## 📚 Documentation

- [Full Zero Trust API Blueprint (29 sections)](docs/blueprint.md) – technical, operational, and governance controls.
- [Development Guide](guide.md) – incremental steps, TDD practices, and project setup.

---

## 📡 OpenAPI Specification

This project uses **built-in OpenAPI** (Microsoft.AspNetCore.OpenApi). No Swashbuckle or Swagger UI is included by default.

- OpenAPI JSON document: `http://localhost:5000/openapi/v1.json`
- To explore the API, you can:
  - Use [Swagger UI](https://swagger.io/tools/swagger-ui/) locally by pointing it to the `/openapi/v1.json` URL.
  - Use Visual Studio's built-in HTTP file or `.http` requests.
  - Use tools like Postman or Insomnia with the OpenAPI import.

To add a Swagger UI, install `Swashbuckle.AspNetCore` and add `app.UseSwagger(); app.UseSwaggerUI();` in `Program.cs`. However, for a minimal Zero Trust baseline, we keep the default lightweight OpenAPI.

---

## 🧪 Testing

Run all tests (unit + integration):

```shell
dotnet test
```

Tests are written using **xUnit** and follow **TDD** (red-green-refactor). Each user story includes Gherkin scenarios that are turned into automated tests before implementation.

---

## 🐳 Docker Compose Details

The `docker-compose.yml` launches the API on port `5000` (host) → `8080` (container).  
To change the port, edit the `ports` mapping.

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

```text
zero-trust-api-blueprint/
├── src/
│   └── ZeroTrustAPI.Api/          # Main API project (controllers only)
│       ├── Controllers/            # HealthController, AuthController, etc.
│       ├── Program.cs              # Entry point (built‑in OpenAPI)
│       └── ZeroTrustAPI.Api.csproj
├── tests/
│   └── ZeroTrustAPI.Tests/         # xUnit test project (TDD)
├── docs/
│   └── blueprint.md                # 29‑section Zero Trust blueprint
├── Dockerfile                      # Multi‑stage build for .NET 11 preview
├── docker-compose.yml
├── guide.md                        # Incremental development guide
└── README.md
```

---

## 🛣️ Roadmap

The implementation follows the blueprint's priority matrix:

- **Critical (MVP):** Identity, MFA, JWT, fine‑grained authorization, logging, secrets, data protection, IAM governance, SLOs.
- **High:** API gateway, context‑aware security, microservices security, rate limiting, testing, CI/CD, incident response, policy as code, data classification.
- **Medium:** Request validation, compliance, observability, supply chain security, threat intelligence, UEBA, documentation.
- **Low:** External penetration testing, cost/business alignment.

Current status: ✅ **Initialization complete** – `/health` endpoint working with controllers + Docker.  
Next: **Story 1.1 – MFA Enforcement (JWT + TOTP)**.

---

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for TDD requirements, signed commits, two‑person review, and security policies.

---

## 📄 License

Proprietary – internal enterprise use.

---

**Built with .NET 11 preview, Docker, and Zero Trust principles.**
