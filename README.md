# Social Media App

Full-stack social media application built as a multi-service .NET 8 project. The solution includes authentication, people management, posts, and an MVC frontend. It can run both locally and in Docker.

## Project Overview

This project was created to demonstrate practical backend and full-stack skills in a small microservices-style architecture:

- authentication with JWT
- modular API separation by domain
- Entity Framework Core with SQL Server
- Docker-based local infrastructure
- ASP.NET Core MVC frontend consuming internal APIs

The application supports a simple social media flow:

- user registration and login
- browsing other users
- adding and removing friends
- creating and deleting posts
- filtering posts by all posts, own posts, and friends' posts

## Architecture

The solution is split into four applications:

- `IdentityApi` - registration, login, JWT token generation
- `PeopleApi` - users and friendships
- `PostApi` - post management and filtering
- `SocialMediaApp` - ASP.NET Core MVC frontend

Database:

- SQL Server 2022
- Entity Framework Core migrations applied automatically on startup

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Docker Compose
- AutoMapper
- JWT Bearer Authentication
- BCrypt password hashing

## Key Features

### Authentication

- register new user account
- login with JWT-based authentication
- protected endpoints secured with bearer token

### Social Graph

- list users
- list available users to add
- add friend
- remove friend
- view current friends

### Posts

- create post
- delete own post
- view all posts
- view own posts
- view friends' posts

## Running the Project

### Configuration

1. Copy `.env.example` to `.env`
2. Fill in your local values in `.env`
3. Use the same `.env` file for both Docker and local startup

### Recommended: Full Docker setup

This is the easiest way to run the whole application and the recommended option for reviewing the project.

Run the full stack with Docker:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

Docker ports:

- PeopleApi: `http://localhost:5001`
- Frontend: `http://localhost:5002`
- IdentityApi: `http://localhost:5003`
- PostApi: `http://localhost:5004`

### Option 2: Local APIs + SQL Server in Docker

1. Prepare local config:

```powershell
Copy-Item .env.example .env
```

2. Start SQL Server:

```powershell
docker compose up -d mssql
```

3. Start all APIs and the frontend from the repository root:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\Start-Local.ps1
```

If PowerShell blocks script execution on Windows, use the command above in the same terminal session. The `Process` scope changes the policy only for the current PowerShell window.

4. Open the frontend:

```text
http://localhost:5065
```

Local development ports:

- IdentityApi: `http://localhost:5057`
- PeopleApi: `http://localhost:5096`
- PostApi: `http://localhost:5035`
- Frontend: `http://localhost:5065`

## Repository Structure

```text
IdentityApi/      authentication and users
PeopleApi/        user relationships and friendships
PostApi/          post management
SocialMediaApp/   MVC frontend
docker-compose.yml
TTSAW_APP_SocialMedia.sln
```

## What This Project Demonstrates

- designing a simple service-based backend split by domain
- configuring API-to-frontend communication in development and Docker environments
- handling authentication and authorization with JWT
- working with EF Core migrations and SQL Server
- building a complete end-to-end application
 - keeping secrets out of the repository while preserving a simple startup flow

## Local Secret Setup

- `.env.example` contains the required keys and example values
- `.env` is ignored by Git and should contain your real local values
- Docker reads `.env` during `docker compose up`
- local startup reads `.env` through `scripts/Start-Local.ps1`


