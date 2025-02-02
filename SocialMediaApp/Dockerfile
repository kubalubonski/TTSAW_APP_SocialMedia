FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./SocialMediaApp.csproj ./
RUN dotnet restore "SocialMediaApp.csproj"
COPY . .
RUN dotnet build "SocialMediaApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialMediaApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialMediaApp.dll"]
