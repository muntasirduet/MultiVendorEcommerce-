# MultiVendorEcommerce

A microservices-based multi-vendor e-commerce platform built with .NET 9, Clean Architecture, and CQRS.

## Services

| Service | Port | Status |
|---------|------|--------|
| Catalog API | 5010 | ✅ Implemented |
| PostgreSQL | 5432 | ✅ Infrastructure |
| Redis | 6379 | ✅ Infrastructure |
| RabbitMQ | 5672 / 15672 | ✅ Infrastructure |
| Keycloak | 8080 | ✅ Infrastructure |
| Seq (Logging) | 5341 | ✅ Infrastructure |
| Elasticsearch | 9200 | ✅ Infrastructure |

## Architecture

Each service follows **Clean Architecture** with:
- **Domain** – Entities, domain events, enums
- **Application** – CQRS commands/queries via MediatR, FluentValidation, repository interfaces
- **Infrastructure** – EF Core (PostgreSQL), repository implementations
- **API** – ASP.NET Core controllers, JWT auth via Keycloak, Swagger

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker](https://www.docker.com/) & Docker Compose

### 1. Configure environment

```bash
cp .env.example .env
# Edit .env with your secrets
```

### 2. Start infrastructure

```bash
docker compose up postgres redis rabbitmq keycloak seq elasticsearch -d
```

### 3. Run Catalog API locally

```bash
cd src/Services/Catalog/Catalog.API
dotnet run
```

The API will be available at `http://localhost:5010`.  
Swagger UI: `http://localhost:5010/swagger`

### 4. Run everything with Docker

```bash
docker compose up --build -d
```

## API Endpoints

### Products
| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/api/v1/products` | Public | List products (paginated, filterable) |
| GET | `/api/v1/products/{id}` | Public | Get product by ID |
| POST | `/api/v1/products` | vendor/admin | Create product |
| PUT | `/api/v1/products/{id}` | vendor/admin | Update product |
| DELETE | `/api/v1/products/{id}` | vendor/admin | Deactivate product |

### Categories
| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/api/v1/categories` | Public | Get category tree |
| GET | `/api/v1/categories/{id}` | Public | Get category by ID |
| POST | `/api/v1/categories` | admin | Create category |
| PUT | `/api/v1/categories/{id}` | admin | Update category |

## Authentication

The API uses [Keycloak](https://www.keycloak.org/) for OAuth2/OIDC.  
Realm: `ecommerce` | Roles: `admin`, `vendor`, `customer`

Obtain a token from: `http://localhost:8080/realms/ecommerce/protocol/openid-connect/token`
