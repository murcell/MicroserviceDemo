# MicroserviceDemo

Simple microservice architecture.

[![C#](https://img.shields.io/badge/Language-C%23-blue)](https://learn.microsoft.com/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-512BD4)](https://learn.microsoft.com/aspnet/core/)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED)](https://www.docker.com/)
[![SQL Server](https://img.shields.io/badge/DB-SQL%20Server-CC2927)](https://www.microsoft.com/sql-server)

## Tech Stack
- **.NET** (target framework as defined in each `.csproj`)  
- **ASP.NET Core** (Web API, MVC/Razor)
- **IdentityServer** (authentication/authorization)
- **API Gateway** (gateway project)
- **Docker & Docker Compose**
- **Databases:** SQL Server
- **UI:** ASP.NET Core Web (Razor/MVC), **SCSS**, **HTML**, **CSS**, **JavaScript**
- **Scripting:** PowerShell
- **Dockerfile**
- **Shared** class library (common utilities)

## Solution Layout
- `FreeCourse.Gateway` — API Gateway  
- `IdentityServer/FreeCourse.IdentityServer` — Identity Server  
- `Fronttends/FreeCourse.Web` — Web UI (Razor/MVC)  
- `Services` — Microservices  
- `Shared/FreeCourse.Shared` — Shared library  
- `MicroServiceDatabases` — Database assets/config  
- `docker-compose.yml` — Orchestration

## Notes
- Replace **.NET target framework** text above with the actual TFM from your project files (e.g., `net6.0`, `net7.0`, `net8.0`).
