# ElGhoul — Frozen Foods E-Commerce Backend API

**ElGhoul** is the backend API for a frozen-foods retail chain (fish, meat, poultry, and other frozen products) operating multiple physical branches. It powers two client applications from a single API: a **customer storefront** (mobile/web) and an **internal admin dashboard**.

Customers browse products by category and brand, view active discounts and bundle offers, add items to a cart, and place pickup orders from their preferred branch — orders are paid for and collected in person, with no online payment involved. Admins manage branches, catalog, pricing, offers, cross-branch inventory, employees, and view real-time analytics from a dedicated dashboard.

Built with **ASP.NET Core Web API** following strict **Clean Architecture**, backed by **Entity Framework Core** on **Azure SQL Database**, with **Azure Blob Storage** for images, **SendGrid** for transactional email, and **Google OAuth** for social login.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Overview](#api-overview)
- [Authentication Model](#authentication-model)
- [Database](#database)
- [Deployment](#deployment)
- [Project Timeline](#project-timeline)
- [Roadmap](#roadmap)
- [License](#license)

---

## Features

**Customer app**
- Email/password registration & login, Google Sign-In, forgot/reset password
- Browse products by category (hierarchical main/sub-categories) and brand
- Active offers & bundle deals, product reviews & ratings, wishlist
- Cart and checkout, with stock checked against the customer's preferred branch
- Order history, order tracking, order cancellation (while still pending)
- Editable profile (name, phone, preferred branch)

**Admin dashboard**
- Branches, categories, brands, and products — full CRUD with image uploads
- Offers: percentage discounts and fixed bundle prices, with analytics
- Cross-branch inventory: stock levels, low-stock alerts, branch-to-branch transfers, history, CSV export
- Pricing management with margin and revenue-tier analytics
- Employee records (roster, shifts, roles)
- Customer management (search, stats, top spenders, enable/disable accounts)
- Order status management
- Dashboard overview: revenue trend, category performance, operational alerts

**Platform**
- 99 REST endpoints across 14 functional modules
- JWT authentication with distinct Customer / Admin token claims and authorization policies
- No public admin sign-up — the first super-admin is seeded once; every admin after that is created by an existing admin
- Centralized validation (FluentValidation) and global exception handling

---

## Tech Stack

| Category | Technology |
|---|---|
| Language / Runtime | C# / .NET (ASP.NET Core Web API) |
| Architecture | Clean Architecture (Domain, Application, Infrastructure, API) |
| ORM | Entity Framework Core (Code-First + Migrations) |
| Database | Azure SQL Database (SQL Server) |
| Auth | JWT Bearer tokens, custom Authorization Policies |
| Password hashing | BCrypt.Net |
| Validation | FluentValidation |
| Social login | Google Sign-In (`Google.Apis.Auth`) |
| Email | SendGrid |
| File storage | Azure Blob Storage |
| API docs | Swagger / Swashbuckle |
| Hosting | Azure App Service |

---

## Architecture

Four independent projects, each with its own `.csproj`, so dependency direction is enforced by the compiler rather than convention. Every reference points inward — `Domain` depends on nothing, and nothing depends on `API`.

```
API  →  Application  →  Domain
 ↑
Infrastructure  →  Application  →  Domain
```

| Layer | Responsibility |
|---|---|
| **Domain** | Entities and enums only. No EF Core, no ASP.NET, no external dependencies. |
| **Application** | DTOs, FluentValidation validators, service classes (use cases), and interfaces for repositories/security/file storage/email — implemented elsewhere. |
| **Infrastructure** | EF Core `DbContext` + configurations, repository implementations, JWT/BCrypt, Azure Blob Storage, SendGrid, Google token verification. |
| **API** | Controllers, global exception middleware, and `Program.cs` (DI, JWT, authorization policies, CORS, Swagger). |

**Authorization model:** no role table, no admin hierarchy. Every endpoint is Public, `CustomerOnly`, or `AdminOnly`, enforced by two authorization policies that check a `type` claim in the JWT. Customers and Admins live in fully separate tables — a bug in the public registration endpoint can never grant admin access.

---

## Project Structure

```
ElGhoul/
├── Domain/                # Entities, enums — zero dependencies
├── Application/           # DTOs, Services, Validators, Interfaces
├── Infrastructure/        # EF Core, Repositories, JWT, Blob Storage, SendGrid, Google Auth
├── API/                   # Controllers, Middlewares, Program.cs
└── ElGhoul.slnx
```

---

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- SQL Server / SQL Server Express
- An Azure Storage account (Blob Storage) — or a local stub for development
- A [SendGrid](https://sendgrid.com) account
- A [Google Cloud](https://console.cloud.google.com) OAuth 2.0 Client ID

### Setup

```bash
# 1. Clone
git clone https://github.com/<your-username>/ElGhoul.git
cd ElGhoul

# 2. Restore & build
dotnet restore
dotnet build

# 3. Apply database migrations
dotnet ef database update --project Infrastructure --startup-project API

# 4. Run
dotnet run --project API
```

On first run, a database seeder automatically creates the first **super-admin** account — no manual SQL insert needed. Any admin after that must be created by an authenticated admin via `POST /api/admin/auth/add`.

Once running, open Swagger UI at the root URL to explore and test every endpoint interactively.

---

## Configuration

All settings live in `API/appsettings.json` for local development. **Never commit real secrets** — in production these are injected as environment variables / Azure App Service Application Settings using the double-underscore convention (e.g. `Jwt__Key`).

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ElGhoulDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<min-32-char-secret>",
    "Issuer": "ElGhoul",
    "Audience": "ElGhoul",
    "ExpireDays": "7"
  },
  "SendGrid": {
    "ApiKey": "<your-sendgrid-api-key>",
    "FromEmail": "<verified-sender-email>",
    "FromName": "ElGhoul"
  },
  "Google": {
    "ClientId": "<your-google-oauth-client-id>"
  },
  "AzureBlobStorage": {
    "ConnectionString": "<your-storage-connection-string>",
    "ContainerName": "elghoul-media"
  }
}
```

---

## API Overview

99 endpoints across 14 modules.

| Module | Example endpoints | Access |
|---|---|---|
| Auth | `POST /api/customer/auth/register`, `POST /api/customer/auth/google`, `POST /api/admin/auth/login` | Public / Admin |
| Customers | `GET /api/customers`, `GET /api/customers/stats` | Admin |
| Branches | `GET /api/branch`, `POST /api/branch`, `GET /api/branch/stats` | Public / Admin |
| Categories | `GET /api/category` (hierarchical), `POST /api/category` | Public / Admin |
| Products | `GET /api/product/App/filter`, `POST /api/product` | Public / Admin |
| Offers | `GET /api/offer/active`, `POST /api/offer/{id}/products` | Public / Admin |
| Orders | `POST /api/order`, `GET /api/order/my`, `PUT /api/order/{id}/status` | Customer / Admin |
| Inventory | `GET /api/branchinventory/grid`, `POST /api/branchinventory/transfer` | Admin |
| Cart / Wishlist | `POST /api/cart`, `POST /api/wishlist/{productId}` | Customer |
| Reviews | `POST /api/review`, `GET /api/review/product/{id}` | Customer / Public |
| Dashboard | `GET /api/dashboard/overview` | Admin |
| Employees | `GET /api/employee`, `POST /api/employee` | Admin |

---

## Authentication Model

| | Customer token | Admin token |
|---|---|---|
| Identity claim | `Customer_Id` | `Admin_Id` |
| `type` claim | `"customer"` | `"admin"` |
| Extra claim | — | `branchId` (or `null` for a super-admin who sees every branch) |

- Passwords: min. 8 characters, at least one uppercase letter, one digit — hashed with BCrypt.
- Google Sign-In is a two-step flow for brand-new users: step 1 sends only the Google `idToken`; if the account is new and missing a phone number / preferred branch, the API responds with `requiresAdditionalInfo: true` and no token; step 2 resends the **same** `idToken` plus the missing fields to complete registration.
- Forgot password: a 6-digit OTP is emailed via SendGrid, verified, then the password is reset — all three steps are separate endpoints.

---

## Database

15 core entities, including a self-referencing `Category` tree (main/sub-categories) and composite-key join entities (`BranchInventory`, `WishList`, `OfferProduct`) where the relationship itself is the identity.

---

## Deployment

Hosted on **Azure App Service**, with **Azure SQL Database** and **Azure Blob Storage**. Migrations must be applied to both the local and the Azure database whenever the schema changes:

```bash
dotnet ef database update --project Infrastructure --startup-project API                              # local
dotnet ef database update --project Infrastructure --startup-project API --connection "<azure-conn>"   # Azure
```

---

## Project Timeline

Built in **4 weeks**, June 20 – July 17, 2026:

| Week | Focus |
|---|---|
| 1 | Clean Architecture setup, Customer/Admin auth, JWT, Google Sign-In, password reset |
| 2 | Branches, hierarchical Categories, Brands, Products, first Azure deployment |
| 3 | Offers, Cart, Orders, Wishlist, Reviews, cross-branch inventory, Employees |
| 4 | Admin dashboard, analytics, customer management, final Azure sync & hardening |

---

## Roadmap

- [ ] Facebook login (schema field already in place)
- [ ] Push notifications for order status changes
- [ ] Automated unit / integration tests
- [ ] API rate limiting

*Online payment is intentionally out of scope — all orders are pay-on-pickup at the branch.*

---

## License

This project is currently private / unlicensed. Add a license file here if the project is made public.

---

**Author:** Ibrahim Hamdy — Backend Developer
