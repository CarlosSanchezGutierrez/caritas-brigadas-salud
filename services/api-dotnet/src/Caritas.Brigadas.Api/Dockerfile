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

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=0

EXPOSE 8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "Caritas.Brigadas.Api.dll"]
