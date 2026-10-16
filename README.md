# RaceDay – Event Management System (PROG6212 PoE)

https://github.com/ST10468948/PROG6212_Part_1.git

RaceDay is a cloud-ready platform engineered to streamline event administration for South African road running, walking, and cycling events—ranging from community park runs to major regional races like the Comrades Marathon, Soweto Marathon, and Cape Town Cycle Tour. 

The system transitions traditional spreadsheet management into a secure, tiered architecture consisting of an ASP.NET Core Web API, Entity Framework Core Code-First data persistence, and automated xUnit integration testing[cite: 1].

---

## Part 2 Architectural Overview

Part 2 introduces a multi-project .NET 8 solution implementing a RESTful API with Code-First database persistence and automated CI/CD:

* **`RaceDay.Contracts`**: Class library containing POCO Data Transfer Objects (DTOs) with automatic properties for decoupling domain entities from network serialization.
* **`RaceDay.Api`**: ASP.NET Core Web API implementing RESTful controllers, Entity Framework Core `DbContext`, BCrypt password hashing, and custom session-based `AuthorizeRole` action filters.
* **`RaceDay.Api.Tests`**: Automated test suite executing xUnit integration tests with `WebApplicationFactory` and an in-memory database to validate authentication and role enforcement without affecting SQL Server.

---

## Domain Entities & Database Schema

The database model is managed via EF Core Code-First migrations applied to SQL Server, enforcing key constraints and referential integrity:

1. **`Roles`**: Defines system authorization tiers (`Organiser` and `Participant`).
2. **`Users`**: Stores participant and organizer accounts with unique email constraints and BCrypt password hashes.
3. **`Events`**: Manages event details, dates, categories, locations, and distances.
4. **`Categories`**: Defines distance, age, or competitive tiers within an event.
5. **`Enrolments`**: Handles registrations linking a participant, event, and category, preventing duplicate event entries.
6. **`Results`**: Stores finishing times, overall rankings, and total field sizes recorded by the event organizer.

---

## REST API Endpoint Summary

| Controller | Method | Endpoint | Role Restriction | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/auth/register` | Public | Registers a new account as Organiser or Participant. |
| **Auth** | `POST` | `/api/auth/login` | Public | Validates credentials and sets server session cookies. |
| **Auth** | `POST` | `/api/auth/logout` | Authenticated | Clears the active session. |
| **Profile** | `GET` | `/api/profile/me` | Authenticated | Retrieves current logged-in profile details. |
| **Profile** | `PUT` | `/api/profile/me` | Authenticated | Updates profile full name and photo URL. |
| **Events** | `GET` | `/api/events` | Public | Retrieves all active events. |
| **Events** | `GET` | `/api/events/{id}` | Public | Retrieves specific event details. |
| **Events** | `POST` | `/api/events` | `Organiser` | Creates a new race event. |
| **Events** | `PUT` | `/api/events/{id}` | `Organiser` | Updates an event owned by the logged-in organiser. |
| **Events** | `DELETE` | `/api/events/{id}` | `Organiser` | Deletes an owned event. |
| **Categories** | `GET` | `/api/events/{id}/categories` | Public | Retrieves categories for an event. |
| **Categories** | `POST` | `/api/events/{id}/categories` | `Organiser` | Adds a category to an event. |
| **Enrolments** | `POST` | `/api/enrolments` | `Participant` | Enrols into an event and category. |
| **Enrolments** | `GET` | `/api/enrolments/mine` | `Participant` | Lists all personal event enrolments. |
| **Enrolments** | `GET` | `/api/enrolments/event/{id}` | `Organiser` | Lists all participant entries for an event. |
| **Results** | `POST` | `/api/results` | `Organiser` | Records finishing position and race time. |
| **Results** | `GET` | `/api/results/mine` | `Participant` | Retrieves race history and finishing results. |

---

## Getting Started & Local Setup

### Prerequisites
* Visual Studio 2022 (v17.8 or higher) with **ASP.NET and web development** workload.
* .NET 8.0 SDK.
* Microsoft SQL Server / LocalDB / SQL Server Express and SSMS.

### Database Migration
1. Open `RaceDay.sln` in Visual Studio 2022.
2. Verify the connection string in `RaceDay.Api/appsettings.json` points to your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=RaceDayDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
   }

   Open Package Manager Console (Tools -> NuGet Package Manager -> Package Manager Console).

Set the Default project to RaceDay.Api and apply migrations:

PowerShell
Update-Database
Running the API
Set RaceDay.Api as the startup project in Solution Explorer.

Press F5 or Ctrl + F5.

Swagger UI will launch in your browser at https://localhost:<port>/swagger to interactively test endpoints.

Automated Testing & CI/CD Pipeline
The project includes automated integration tests under RaceDay.Api.Tests covering critical authentication and authorization scenarios:

User registration validation (201 Created).

Login credential verification and rejection (401 Unauthorized).

Unauthenticated resource access rejection (401 Unauthorized).

Role authorization boundary testing (verifying Participants receive 403 Forbidden when attempting Organiser actions).

Organiser operational authorization verification (201 Created).

To run tests locally via the .NET CLI:

Bash
dotnet test RaceDay.sln
GitHub Actions CI Workflow
Every push and pull request to the main branch triggers .github/workflows/dotnet-ci.yml, which provisions an Ubuntu runner, restores NuGet packages, builds the multi-project solution in Release configuration, and executes all xUnit integration tests against an in-memory database instance.

Part 2 Deliverables & Documentation
Source Code: Fully implemented .NET 8 Web API, POCO DTO contracts, and xUnit integration test suite.

Database Setup: EF Core Code-First migration files located in RaceDay.Api/Migrations/.

CI/CD Pipeline: GitHub Actions workflow file located at .github/workflows/dotnet-ci.yml.

Demonstration Video: Unlisted YouTube presentation link: [INSERT YOUR YOUTUBE URL HERE]

[cite: 1]

References


GitHub, 2024. GitHub Actions Documentation: Building and testing .NET. Available at: https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net .

Microsoft, 2024. Create Data Transfer Objects (DTOs). Microsoft Learn. Available at: https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5 .

Microsoft, 2024. Integration tests in ASP.NET Core. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests .

Microsoft, 2024. Overview of Entity Framework Core. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/ef/core/ .

Microsoft, 2024. Session and state management in ASP.NET Core. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state .

OpenAI, 2024. GitHub Copilot (Version 1.84) [Large language model]. Available through Visual Studio 2022 integration. Used as a peer programmer for code structure recommendations, debugging EF Core model key constraints, and generating xUnit integration test scaffolding.

