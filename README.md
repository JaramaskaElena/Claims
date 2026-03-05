# Claims API

This project implements a **Claims Management API** using a **layered (clean) architecture** and follows **SOLID principles** for maintainability, testability, and separation of concerns.

## Architecture

The solution is divided into multiple projects/layers:

- **Claims.API (Presentation Layer)** – Contains the controllers and handles HTTP requests/responses.
- **Claims.Application (Application/Services Layer)** – Implements business logic and application services.
- **Claims.Domain (Domain Layer)** – Contains entities, enums, and domain events.
- **Claims.Infrastructure (Infrastructure Layer)** – Handles data access (repositories, EF Core) and integration with external services.

This separation ensures that each layer has a **single responsibility** and can evolve independently.

## Event-Driven Architecture

The API uses an **event-driven approach**:

- Each significant action (e.g., ClaimCreated, CoverCreated) triggers a **domain event**.
- Events are dispatched asynchronously via an `IEventDispatcher` to prevent **blocking the main workflow**.
- This allows the system to scale and react to changes without waiting for long-running tasks, improving responsiveness and decoupling components.

## Validation & Middleware

- All business validation is handled inside the **Application layer** (e.g., validating `DamageCost`, cover period, or insurance limits).
- Exceptions and validation errors are handled globally by a **custom middleware** in the API:
  - Ensures consistent **HTTP response codes** for errors.
  - Centralizes exception handling so controllers remain clean and focused on handling requests.

## Other Highlights

- **SOLID Principles**: Each layer and class adheres to single responsibility, open/closed, and dependency inversion principles.
- **Unit Tests**: Services are fully tested, including validation rules and premium calculations.
- **EF Core Code-First**: Database is managed with migrations, supporting a flexible and maintainable schema.
- **Deterministic Tests**: Date-related logic is tested reliably to prevent flakiness in CI pipelines.

This architecture ensures a **maintainable, scalable, and responsive system**, following best practices for modern .NET applications.

