# AGENT.md — Grab&Go Microservices (MVP)

> This file is the authoritative architecture reference for AI agents (Claude Code, Copilot, etc.) and new engineers working on this codebase. Read it fully before making any changes.

---

## Project overview

**Grab&Go** is a food & grocery pickup platform built as a distributed system. For the MVP, we focus on **3 core microservices**. The backend is written in **C# / .NET 8** using **ASP.NET Core** Web APIs. Services communicate asynchronously via **RabbitMQ** (MassTransit) and synchronously through the **Ocelot API Gateway**.

## Service documentation URL

All service documentation is unified under a single URL:
`/docs/services`

### Service map (MVP)

| Service | Responsibility | Database | Key patterns |
|---------|---------------|----------|--------------|
| **Order** | Order lifecycle, payment (module) | PostgreSQL | CQRS, Saga |
| **Catalog** | Products, menus, pricing, stores, stock levels | PostgreSQL | CQRS |
| **Identity** | Auth, JWT issuance, refresh tokens, roles | PostgreSQL | ASP.NET Core Identity |

> **Out of scope for MVP:** Notification and Analytics services are deferred to post-MVP phase.

---

## Repository layout

```
GrabAndGo.sln
├── src/
│   ├── gateway/                          # Ocelot API Gateway — single entry point
│   ├── services/
│   │   ├── Order/                        # Core transactional service
│   │   │   └── Modules/Payment/          # Stripe integration — module, not a service
│   │   ├── Catalog/                      # Products, menus, pricing
│   │   │   ├── Modules/Inventory/        # Stock levels — module inside Catalog
│   │   │   └── Modules/Store/            # Locations, hours, slots — module inside Catalog
│   │   └── Identity/                     # Auth — JWT issuance, refresh, roles
│   └── BuildingBlocks/
│       └── GrabAndGo.BuildingBlocks/     # Shared internal NuGet package
├── tests/
│   ├── GrabAndGo.Order.UnitTests/
│   ├── GrabAndGo.Catalog.UnitTests/
│   └── GrabAndGo.Identity.UnitTests/
├── docker-compose.yml                    # Full local stack (MVP)
└── docker-compose.override.yml           # Dev overrides
```

---

## Service anatomy

Every service follows the same four-project vertical slice:
`GrabAndGo.{Name}.API`, `GrabAndGo.{Name}.Application`, `GrabAndGo.{Name}.Domain`, `GrabAndGo.{Name}.Infrastructure`.

---

## Tech stack reference (MVP)

| Concern | Library / Tool | Version |
|---------|---------------|---------|
| Runtime | .NET | 8.0 |
| Web API | ASP.NET Core | 8.0 |
| Mediator | MediatR | 12.x |
| SQL database | PostgreSQL (Order, Identity) | 16 |
| Document store | MongoDB (Catalog) | 7.x |
| Message broker | RabbitMQ via MassTransit | 8.x |
| API Gateway | Ocelot | 23.x |
| Auth | ASP.NET Core Identity + JWT Bearer | — |

---

## Local development

### Prerequisites

- .NET 8 SDK
- Docker Desktop

### Start the MVP stack

```bash
docker-compose up -d
```

Starts: Order, Catalog, Identity + Ocelot gateway, RabbitMQ, PostgreSQL, and MongoDB.

---

## Key architectural decisions (ADR index)

| # | Decision | Status |
|---|----------|--------|
| 001 | Database-per-service — no shared schemas | Accepted |
| 002 | MassTransit over raw RabbitMQ client | Accepted |
| 003 | Outbox pattern for Order, Catalog, Identity | Accepted |
| 004 | Contracts published as NuGet, not shared projects | Accepted |
| 005 | Ocelot as API Gateway | Accepted |
| 006 | Simplified MVP: Removed Redis, EventStoreDB, and Observability stack | Accepted |
