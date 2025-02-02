FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./PostApi.csproj ./
RUN dotnet restore "PostApi.csproj"
COPY . .
RUN dotnet build "PostApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostApi.dll"]
