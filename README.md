# Social Media App

A simple full-stack social media project built with .NET 8 and a multi-service architecture. It includes authentication, people management, posts, and an ASP.NET Core MVC frontend.

## Services

- `IdentityApi` - registration, login, JWT generation
- `PeopleApi` - users and friendships
- `PostApi` - creating, deleting, and filtering posts
- `SocialMediaApp` - ASP.NET Core MVC frontend
- `SQL Server` - database used by the services

## Features

- user registration and login
- JWT-based authentication
- browsing users
- adding and removing friends
- creating and deleting your own posts
- filtering posts: all, mine, friends'

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Docker Compose
- AutoMapper
- BCrypt

## Run The Project

First, create your local config file:

```powershell
Copy-Item .env.example .env
```

Fill in the values in `.env`. The same file is used for both Docker and local startup.

### Option 1: Run everything in Docker

This is the simplest way to start the full application:

```powershell
docker compose up --build
```

Docker ports:

- Frontend: `http://localhost:5002`
- IdentityApi: `http://localhost:5003`
- PeopleApi: `http://localhost:5001`
- PostApi: `http://localhost:5004`

### Option 2: Run locally with SQL Server in Docker

Start the database:

```powershell
docker compose up -d mssql
```

Then start all services from the repository root:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\Start-Local.ps1
```

Local ports:

- Frontend: `http://localhost:5065`
- IdentityApi: `http://localhost:5057`
- PeopleApi: `http://localhost:5096`
- PostApi: `http://localhost:5035`

## Repository Structure

```text
IdentityApi/      authentication
PeopleApi/        users and friendships
PostApi/          posts
SocialMediaApp/   MVC frontend
docker-compose.yml
```

## Purpose

This project demonstrates:

- splitting a backend by domain
- communication between frontend and APIs
- practical use of JWT and EF Core
- a complete flow from login to posts and friendships


