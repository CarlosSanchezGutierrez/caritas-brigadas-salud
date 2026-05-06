# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj src/Caritas.Brigadas.Api/
COPY src/Caritas.Brigadas.Application/Caritas.Brigadas.Application.csproj src/Caritas.Brigadas.Application/
COPY src/Caritas.Brigadas.Contracts/Caritas.Brigadas.Contracts.csproj src/Caritas.Brigadas.Contracts/
COPY src/Caritas.Brigadas.Domain/Caritas.Brigadas.Domain.csproj src/Caritas.Brigadas.Domain/
COPY src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj src/Caritas.Brigadas.Infrastructure/

RUN dotnet restore src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

COPY . .

RUN dotnet publish src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

LABEL org.opencontainers.image.title="Caritas Brigadas de Salud API"
LABEL org.opencontainers.image.description="ASP.NET Core API for Caritas health brigades operations."
LABEL org.opencontainers.image.vendor="Caritas Brigadas de Salud"
LABEL org.opencontainers.image.source="https://github.com/CarlosSanchezGutierrez/caritas-brigadas-salud"

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 8080

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl --fail --silent --show-error http://localhost:8080/health/live || exit 1

USER $APP_UID

ENTRYPOINT ["dotnet", "Caritas.Brigadas.Api.dll"]
