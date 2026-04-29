# Learnings — hexagonal-architecture-doc-main (2026-04-29)

## 1. Reparación de la solución

### global.json y SDK pinning
- El campo `rollForward` con valor `disable` rompe el build si la SDK exacta no está instalada.
- Solución: cambiar a `latestPatch` para tolerar versiones de parche más recientes.

```json
{ "sdk": { "version": "9.0.313", "rollForward": "latestPatch" } }
```

### AutoMapper — vulnerabilidad y versión correcta
- `AutoMapper 14.0.0` tiene una vulnerabilidad DoS registrada (GHSA-rvv3-g6hj-g44x).
- Las versiones `15.1.1` y `16.1.1` requieren `Microsoft.Extensions.Options >= 10.0.0` (.NET 10) → incompatibles con net9.
- La versión correcta es **`15.1.3`**: parcheada y compatible con net9.

### SA1516 + IDE0055 — using directives
- StyleCop SA1516 requiere línea en blanco entre grupos de `using` (System.*, GtMotive.*, Microsoft.*).
- Cuando `TreatWarningsAsErrors=true`, tanto SA1516 como IDE0055 se convierten en errores de compilación.
- Fix masivo: `dotnet format src\microservice.sln --diagnostics IDE0055`

### NuGet.config — feeds privados heredados
- Si la máquina tiene un feed privado configurado globalmente (e.g. Nexus), NuGet lo consulta para **todas** las dependencias transitivas de paquetes nuevos que no están en caché.
- Esto produce `NU1301` si el servidor no es accesible.
- Solución: añadir `<clear />` antes del primer `<add>` en `packageSources` para ignorar configuración global:

```xml
<packageSources>
  <clear />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
</packageSources>
```

---

## 2. Patrones identificados en el proyecto

| Patrón | Dónde |
|---|---|
| Hexagonal / Clean Architecture | Separación Domain / ApplicationCore / Infrastructure / Api / Host |
| Use Case (Input/Output ports) | `IUseCase<TInput>`, `IOutputPortStandard<T>`, `IOutputPortNotFound` |
| Presenter | Clases XxxPresenter implementan el puerto de salida y producen `IActionResult` |
| Strategy | `AppTelemetry` vs `NoOpTelemetry` intercambiables por DI |
| Adapter | `LoggerAdapter<T>`, `AppTelemetry` (adaptan contratos externos) |
| Null Object | `NoOpTelemetry` — no hace nada en Development |
| Facade | Métodos de extensión `AddBaseInfrastructure`, `AddApiDependencies`, `WithApiControllers` |
| Builder fluido | `IInfrastructureBuilder` |
| Abstract Factory | `IBusFactory` / `IBus` (stub sin implementar) |
| Options Pattern | `AppSettings`, `MongoDbSettings` vía `IOptions<T>` |
| Marker Interfaces | Interfaces vacías para categorización en DI |
| Mediator | MediatR registrado; handlers pendientes de implementar |
| Chain of Responsibility | Middleware pipeline de ASP.NET Core |
| Exception Filter | `BusinessExceptionFilter` → convierte `DomainException` en 400 |

---

## 3. Microservicio de renting — decisiones de diseño

### Combined Output Port para evitar conflictos de DI
Cuando **dos** casos de uso necesitan `IOutputPortNotFound` (e.g. RentVehicle y ReturnVehicle), registrar ambos en DI genera conflicto de resolución.

Solución: definir una interfaz combinada por caso de uso en ApplicationCore:

```csharp
// En ApplicationCore — un combined port por use case
public interface IRentVehicleOutputPort
    : IOutputPortStandard<RentVehicleOutput>, IOutputPortNotFound { }
```

El presenter implementa la interfaz combinada. En DI se registra dos veces (concrete + interfaz) apuntando a la misma instancia scoped:

```csharp
services.AddScoped<RentVehiclePresenter>();
services.AddScoped<IRentVehicleOutputPort>(sp => sp.GetRequiredService<RentVehiclePresenter>());
```

### Ciclo de vida correcto en repositorios
- **Repositorios en memoria** → `Singleton` (si fueran Scoped, el estado se pierde entre peticiones).
- **Use cases y presenters** → `Scoped` (estado por request).
- **DbContext (EF Core)** → `Scoped` (patrón estándar; los SQL repos también son Scoped).

### Switch in-memory / PostgreSQL sin cambiar el dominio
```csharp
public static IInfrastructureBuilder AddBaseInfrastructure(
    this IServiceCollection services,
    bool isDevelopment,
    string? connectionString = null)
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        services.AddDbContext<RentingDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<IVehicleRepository, SqlVehicleRepository>();
        services.AddScoped<IRentalRepository, SqlRentalRepository>();
    }
    else
    {
        services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
        services.AddSingleton<IRentalRepository, InMemoryRentalRepository>();
    }
}
```

Los tests funcionales llaman con `AddBaseInfrastructure(true)` (sin connection string) y siguen usando la implementación en memoria sin ningún cambio.

---

## 4. Entidades de dominio compatibles con EF Core

Para que EF Core pueda materializar entidades de dominio con propiedades read-only:

1. Cambiar `{ get; }` a `{ get; private set; }` en todas las propiedades.
2. Añadir un constructor `protected` vacío **después** del `public` (SA1202 exige `public` antes de `protected`).
3. Ignorar propiedades calculadas en la configuración Fluent API.

```csharp
public class Rental
{
    public Rental(Guid vehicleId, string customerId) { /* ... */ }

    // SA1202: public members come before protected members
    protected Rental() { }  // ← Para EF Core

    public Guid RentalId { get; private set; }
    public bool IsActive => EndDate == null; // Calculada, no se persiste
}
```

```csharp
// RentalConfiguration.cs
builder.Ignore(r => r.IsActive);
```

---

## 5. Tests — patrones y problemas

### Tres tipos de test y su alcance
| Tipo | Herramienta | Scope |
|---|---|---|
| Unit | xUnit + Moq | Solo la clase, sin dependencias |
| Infrastructure | xUnit + TestServer (Acheve) | Solo el host: routing, model binding, filtros |
| Functional | xUnit + DI real | Desde caso de uso hasta repositorio; sin host HTTP |

### Conflicto xUnit1000 vs CA1515
- `xUnit1000` requiere que las clases de test sean `public`.
- `CA1515` recomienda que los tipos de aplicación sean `internal`.
- Solución: suprimir CA1515 por tipo en `GlobalSuppressions.cs` para las clases de test.

### CA1707 — underscores en nombres de test
- La convención xUnit usa `Método_Condición_Resultado` con underscores.
- `CA1707` lo marca como error si `TreatWarningsAsErrors=true`.
- Suprimir a nivel de módulo en `GlobalSuppressions.cs`:

```csharp
[assembly: SuppressMessage("Naming", "CA1707:...", Justification = "xUnit naming convention.", Scope = "module")]
```

### TestHost — versión debe coincidir con ASP.NET Core
- `Acheve.TestHost 4.0.0` arrastra `Microsoft.AspNetCore.TestHost 8.0.4`.
- Con una API en .NET 9 se produce `InvalidOperationException: ResponseBodyPipeWriter does not implement UnflushedBytes`.
- Solución: añadir `Microsoft.AspNetCore.TestHost` como referencia directa en el csproj del proyecto de test para que la versión 9.x tome precedencia.

### Resolución de DI en tests funcionales
El presenter y el use case deben resolverse desde el **mismo scope** para que el presenter reciba el output del use case:

```csharp
await Fixture.UsingScope(async sp =>
{
    var useCase  = sp.GetRequiredService<IUseCase<AddVehicleInput>>();
    var presenter = sp.GetRequiredService<AddVehiclePresenter>(); // mismo scope
    await useCase.Execute(input);
    // presenter.ActionResult ya tiene el resultado
});
```

---

## 6. Docker

### Dockerfile — buenas prácticas
- Usar imagen base **sin versión específica de SO** (`mcr.microsoft.com/dotnet/aspnet:9.0` en lugar de `9.0.0-noble-amd64`) para mayor compatibilidad.
- El build context debe ser la **raíz del repositorio** para poder copiar `Directory.Build.*` y `NuGet.config`.
- Separar el COPY de `.csproj` del COPY del código fuente para maximizar el cache de layers de Docker.
- No incluir `ARG PersonalAccessToken` ni credenciales en el Dockerfile. Las dependencias deben resolverse solo desde fuentes públicas.
- Usar el puerto `8080` (por defecto en .NET 9) en lugar de `80`.

### docker-compose con PostgreSQL
```yaml
services:
  postgres:
    image: postgres:17-alpine
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U app -d renting"]
      interval: 5s
      retries: 10

  renting-api:
    environment:
      ASPNETCORE_ENVIRONMENT: Development      # evita Azure KeyVault y AppInsights
      ConnectionStrings__Default: "Host=postgres;..."
    depends_on:
      postgres:
        condition: service_healthy             # espera a que Postgres esté listo
```

- Usar `ASPNETCORE_ENVIRONMENT: Development` en Docker para omitir dependencias cloud (KeyVault, AppInsights).
- El switch in-memory/SQL se controla solo con la presencia de `ConnectionStrings__Default`.
- `EnsureCreatedAsync()` en el arranque crea el schema automáticamente.

---

## 7. Reglas de StyleCop relevantes

| Regla | Descripción |
|---|---|
| SA1202 | `public` members deben ir antes de `protected` |
| SA1210 | `using` directives en orden alfabético dentro de cada grupo |
| SA1516 | Línea en blanco entre grupos de `using` |
| IDE0055 | Formato de código — se puede corregir masivamente con `dotnet format` |
| CA1707 | Sin underscores en identificadores (excepto tests xUnit) |
| CA1515 | Tipos públicos en aplicaciones deberían ser internos (conflicto con xUnit1000) |
| CA2234 | Usar `Uri` en lugar de `string` en métodos HTTP |
| xUnit1000 | Las clases de test deben ser `public` |
| xUnit2032 | Usar `IsType` con `exactMatch: false` en lugar de `IsAssignableFrom` |
