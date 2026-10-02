# 🎟️ Ticketing System

A microservices-based ticketing platform built with **.NET 9**, **Docker**, and **SQL Server**. The system handles event management, seat reservation, and booking workflows with JWT-based authentication across all services.

> **Portfolio project** demonstrating microservices architecture, clean code principles, and production-grade patterns.

## System Design/Diagram 
> **Note :** I didn't add some services yet like the payment service and notification service and the rabbitmq message bus and the redis cache.

<img width="5502" height="2907" alt="sys design  net" src="https://github.com/user-attachments/assets/8c4bc6bb-5794-4d62-9d5d-4ffc1c800cc4" />

## 🏗️ Architecture

The system is composed of three independent microservices, each with its own database, communicating via HTTP and secured with JWT tokens.

```text
┌─────────────────────┐
│ Client App │
│ (Swagger / Future) │
└──────────┬──────────┘
│
┌────────────────┼────────────────┐
│ │ │
▼ ▼ ▼
┌──────────────────┐ ┌──────────────┐ ┌──────────────┐
│ Identity.API │ │EventCatalog │ │ Booking.API │
│ │ │ .API │ │ │
│ • Register/Login │ │ • Venues │ │ • Bookings │
│ • JWT issuance │ │ • Events │ │ • Reserve │
│ • BCrypt hashing │ │ • Seats │ │ • Cancel │
└────────┬─────────┘ └──────┬───────┘ └──────┬───────┘
│ │ │
▼ ▼ ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│Ticketing │ │Ticketing │ │Ticketing │
│Identity DB │ │EventCatalog │ │Booking DB │
└──────────────┘ │DB │ └──────────────┘
└──────────────┘

All services share the same JWT signing key for cross-service
token validation. Booking.API forwards the user's token when
calling EventCatalog.API to reserve or release seats.
```


---

## 🚀 Features

### Core Functionality
- **Event & Venue Management** — Admins create venues and events, then bulk-generate seat maps (e.g., 5 rows × 10 seats)
- **Seat Reservation** — Atomic reserve/release of seats with availability tracking
- **Booking Workflow** — Users create bookings, reserve seats, and cancel (which releases seats back)
- **Ownership Enforcement** — Users can only view and cancel their own bookings

### Authentication & Authorization
- **JWT Authentication** — Tokens issued by Identity.API, validated independently by each service
- **Role-Based Access Control** — `[Authorize(Roles = "Admin")]` protects admin-only endpoints
- **BCrypt Password Hashing** — Industry-standard password security
- **Token Delegation** — Booking.API forwards the user's JWT to EventCatalog.API on internal calls

### Developer Experience
- **Clean Architecture** — Domain, Application, Infrastructure, and API layers separated per service
- **Docker Compose** — Full stack runs with a single command
- **Swagger UI** — Interactive API documentation for every service
- **Unit Tests** — xUnit + Moq + FluentAssertions covering core booking logic
- **Entity Framework Core** — Code-first migrations with SQL Server

---

## 🛠️ Tech Stack

| Layer | Technology |
| :--- | :--- |
| **Runtime** | .NET 9 |
| **Web Framework** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core 9 |
| **Database** | SQL Server 2022 |
| **Authentication** | JWT (JSON Web Tokens) |
| **Password Hashing** | BCrypt.Net-Next |
| **API Documentation** | Swashbuckle (Swagger) |
| **Containerization** | Docker + Docker Compose |
| **Testing** | xUnit, Moq, FluentAssertions, EF Core InMemory |
| **Language** | C# 13 |

---

## 📦 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- (Optional) [SQL Server Management Studio](https://aka.ms/ssms) for inspecting the database

### Option 1: Run with Docker (Recommended)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/MoSalah-tech/TicketingSystem.git
   cd TicketingSystem
