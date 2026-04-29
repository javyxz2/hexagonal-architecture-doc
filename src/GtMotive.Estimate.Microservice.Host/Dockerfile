# Build context must be the repo root (hexagonal-architecture-doc-main/).
# docker-compose.yml sets the context automatically.
#
# Manual build:
#   docker build -f src/GtMotive.Estimate.Microservice.Host/Dockerfile -t renting-api .

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy central package management and NuGet config first to maximise layer cache.
COPY Directory.Build.props Directory.Build.props
COPY Directory.Build.targets Directory.Build.targets
COPY NuGet.config NuGet.config

# Copy only .csproj files and restore — changes to source code won't bust this layer.
COPY src/GtMotive.Estimate.Microservice.Domain/GtMotive.Estimate.Microservice.Domain.csproj \
     src/GtMotive.Estimate.Microservice.Domain/
COPY src/GtMotive.Estimate.Microservice.ApplicationCore/GtMotive.Estimate.Microservice.ApplicationCore.csproj \
     src/GtMotive.Estimate.Microservice.ApplicationCore/
COPY src/GtMotive.Estimate.Microservice.Infrastructure/GtMotive.Estimate.Microservice.Infrastructure.csproj \
     src/GtMotive.Estimate.Microservice.Infrastructure/
COPY src/GtMotive.Estimate.Microservice.Api/GtMotive.Estimate.Microservice.Api.csproj \
     src/GtMotive.Estimate.Microservice.Api/
COPY src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj \
     src/GtMotive.Estimate.Microservice.Host/

RUN dotnet restore src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj

# Copy the rest of the source and publish.
COPY src/ src/

RUN dotnet publish src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GtMotive.Estimate.Microservice.Host.dll"]
