# Portfolio Notes

## Before Publishing

1. Remove secrets from the repository
- move SQL passwords and JWT secrets out of `appsettings.json`
- use environment variables, user secrets, or a local `.env` file that is not committed
- keep only placeholder values in the repository

2. Add screenshots or a short demo GIF
- login/register view
- friends/users view
- posts feed with filtering

3. Keep Docker as the primary startup path
- Docker should stay the recommended option for project review
- local startup can stay as a secondary development option

## Planned Improvements

### Frontend

- redesign the UI with stronger themes and more polished screens
- improve consistency of forms, buttons, cards, lists, and post views
- add better empty states, loading states, and error states
- improve responsive behavior on mobile and smaller screens

### Backend and Code Quality

- add global exception handling middleware
- add structured logging and request logging
- add request validation and clearer API error responses
- add automated tests for core flows
- add health checks and basic observability

### Product

- improve user profiles and social features
- add pagination and better filtering
- add demo or seed data for easier presentation

## Secret Management Note

If secrets are moved to environment variables, other people can still run the project as long as the repository includes a clear setup example.

Recommended approach:

1. keep placeholders in tracked config files
2. provide `.env.example` or example values in documentation
3. document which variables must be set before startup
4. use real values only in local environment variables or non-committed files

Example variables to document:

- `ConnectionStrings__DefaultConnection`
- `ConnectionStrings__DockerConnection`
- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`