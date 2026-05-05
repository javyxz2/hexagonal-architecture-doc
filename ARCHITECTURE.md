# Renting Microservice — Documentación técnica

## Índice

1. [Visión general](#1-visión-general)
2. [Arquitectura hexagonal](#2-arquitectura-hexagonal)
3. [Estructura de proyectos](#3-estructura-de-proyectos)
4. [Dominio](#4-dominio)
5. [Casos de uso](#5-casos-de-uso)
6. [API REST](#6-api-rest)
7. [Infraestructura y persistencia](#7-infraestructura-y-persistencia)
8. [Autenticación](#8-autenticación)
9. [Observabilidad](#9-observabilidad)
10. [Cómo ejecutar](#10-cómo-ejecutar)
11. [Tests](#11-tests)

---

## 1. Visión general

Microservicio de gestión de alquiler de vehículos desarrollado en **.NET 9** siguiendo los principios de **arquitectura hexagonal** (también conocida como *ports & adapters*).

Permite:
- Dar de alta vehículos en la flota (con restricción de antigüedad máxima de 5 años).
- Alquilar un vehículo a un cliente (identificado por nombre y DNI).
- Devolver un vehículo alquilado.
- Consultar el catálogo completo y los vehículos disponibles.
- Obtener los detalles de alquiler de un cliente por su DNI.

---

## 2. Arquitectura hexagonal

```
┌─────────────────────────────────────────────────────────┐
│                        API (Driving)                    │
│         Controllers  ──►  Use Cases  ◄──  Output Ports  │
└────────────────────────┬────────────────────────────────┘
                         │  Interfaces (Ports)
┌────────────────────────▼────────────────────────────────┐
│                   Application Core                      │
│   Use Cases · Input/Output DTOs · ITelemetry · ILogger  │
└────────────────────────┬────────────────────────────────┘
                         │  Domain Interfaces
┌────────────────────────▼────────────────────────────────┐
│                       Domain                            │
│        Entities · DomainException · IRepository         │
└────────────────────────┬────────────────────────────────┘
                         │  Implementations (Adapters)
┌────────────────────────▼────────────────────────────────┐
│                  Infrastructure (Driven)                │
│   EF Core · PostgreSQL · Repositories · UnitOfWork      │
└─────────────────────────────────────────────────────────┘
```

El núcleo (Domain + ApplicationCore) **no tiene dependencias externas**. La infraestructura y la API implementan las interfaces definidas en el dominio, nunca al revés.

### Principio de dependencias

```
Api  →  ApplicationCore  →  Domain
Infrastructure  →  Domain
```

---

## 3. Estructura de proyectos

```
src/
├── GtMotive.Estimate.Microservice.Domain/          # Entidades y contratos
│   ├── Entities/
│   │   ├── Vehicle.cs
│   │   ├── Customer.cs
│   │   └── Rental.cs
│   ├── Interfaces/
│   │   ├── IVehicleRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   ├── IRentalRepository.cs
│   │   └── IUnitOfWork.cs
│   └── DomainException.cs
│
├── GtMotive.Estimate.Microservice.ApplicationCore/ # Casos de uso
│   ├── UseCases/
│   │   ├── AddVehicle/
│   │   ├── RentVehicle/
│   │   └── ReturnVehicle/
│   ├── RentingActivitySource.cs                   # ActivitySource compartido
│   └── Interfaces/
│       ├── IUseCase.cs
│       ├── IAppLogger.cs
│       └── ITelemetry.cs
│
├── GtMotive.Estimate.Microservice.Infrastructure/ # Adaptadores driven
│   ├── Persistence/
│   │   ├── RentingDbContext.cs
│   │   ├── UnitOfWork.cs
│   │   └── Configurations/
│   │       ├── VehicleConfiguration.cs
│   │       ├── CustomerConfiguration.cs
│   │       └── RentalConfiguration.cs
│   └── Repositories/
│       ├── VehicleRepository.cs
│       ├── CustomerRepository.cs
│       └── RentalRepository.cs
│
├── GtMotive.Estimate.Microservice.Api/            # Adaptadores driving
│   ├── Controllers/
│   │   └── VehicleController.cs
│   ├── Authentication/
│   │   ├── ApiKeyAuthenticationHandler.cs
│   │   └── ApiKeyAuthenticationOptions.cs
│   └── Presenters/                                # Output port implementations
│
└── GtMotive.Estimate.Microservice.Host/           # Entry point
    ├── Program.cs
    └── Dockerfile

test/
├── unit/                                          # Tests unitarios (xUnit + Moq)
└── infrastructure/                                # Tests de integración (Acheve.TestHost)
```

---

## 4. Dominio

### Entidades

#### `Vehicle`
Representa un vehículo de la flota.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `VehicleId` | `long` | Identificador auto-incremental |
| `Brand` | `string` | Marca |
| `Model` | `string` | Modelo |
| `LicensePlate` | `string` | Matrícula (única) |
| `ManufactureYear` | `int` | Año de fabricación |
| `IsAvailable` | `bool` | Disponible para alquilar |

**Invariantes:** Brand, Model y LicensePlate son obligatorios (no nulos ni vacíos). El año de fabricación no puede ser anterior a 5 años desde la fecha actual (validado en el caso de uso).

**Comportamiento:** `MarkAsRented()` / `MarkAsAvailable()`

---

#### `Customer`
Representa un cliente del servicio.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `CustomerId` | `long` | Identificador auto-incremental |
| `CustomerName` | `string` | Nombre completo |
| `CustomerDni` | `string` | DNI (único, obligatorio) |

**Invariantes:** Name y DNI son obligatorios. El DNI actúa como identificador de negocio único.

---

#### `Rental`
Representa un alquiler activo o completado.

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `RentalId` | `Guid` | Identificador único |
| `VehicleId` | `long` | FK → Vehicle |
| `CustomerId` | `long` | FK → Customer |
| `StartDate` | `DateTime` | Inicio del alquiler |
| `PlannedEndDate` | `DateTime` | Fecha de devolución prevista |
| `ReturnedDate` | `DateTime?` | Fecha real de devolución (null si activo) |

**Comportamiento:** `Complete()` registra la fecha de devolución real.

---

### `DomainException`
Excepción de negocio lanzada por los casos de uso ante reglas violadas. Mapeada a HTTP 400 por `BusinessExceptionFilter`.

---

## 5. Casos de uso

### `AddVehicleUseCase`
Añade un nuevo vehículo a la flota.

**Reglas de negocio:**
- El año de fabricación no puede ser anterior en más de 5 años al año actual.
- La matrícula debe ser única (lanza `DomainException` si ya existe).

**Flujo:**
1. Validar antigüedad del vehículo.
2. Crear entidad `Vehicle`.
3. Persistir vía `IVehicleRepository.AddAsync`.
4. Emitir evento de telemetría `VehicleAdded`.

---

### `RentVehicleUseCase`
Alquila un vehículo a un cliente.

**Reglas de negocio:**
- La fecha de devolución prevista debe ser posterior a la fecha de inicio.
- El vehículo debe existir y estar disponible (`IsAvailable = true`).
- El cliente no puede tener ya un alquiler activo.

**Flujo:**
1. Validar fechas.
2. Obtener vehículo por matrícula.
3. Verificar disponibilidad del vehículo.
4. Obtener o crear cliente por DNI (`FindOrCreateAsync` con manejo de race condition).
5. Verificar que el cliente no tenga alquiler activo.
6. Abrir transacción.
7. Crear `Rental` + marcar vehículo como alquilado.
8. Persistir alquiler y vehículo.
9. Commit.
10. Emitir evento `VehicleRented`.

---

### `ReturnVehicleUseCase`
Registra la devolución de un vehículo.

**Flujo:**
1. Obtener vehículo por matrícula.
2. Obtener alquiler activo del vehículo.
3. Abrir transacción.
4. Completar el alquiler (`rental.Complete()`) + marcar vehículo como disponible.
5. Persistir cambios.
6. Commit.
7. Emitir evento `VehicleReturned`.

---

### Output ports

Los casos de uso no devuelven valores directamente. Llaman a un **output port** (interfaz) con el resultado. Los presenters (en la capa Api) implementan estos puertos y construyen la respuesta HTTP. Este patrón desacopla la lógica de negocio del protocolo de transporte.

---

## 6. API REST

Base URL: `http://localhost:8080`

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/vehicle` | Añadir vehículo |
| `GET` | `/api/vehicle` | Listar todos los vehículos |
| `GET` | `/api/vehicle/available` | Listar vehículos disponibles |
| `POST` | `/api/vehicle/{licensePlate}/rent` | Alquilar vehículo |
| `POST` | `/api/vehicle/{licensePlate}/return` | Devolver vehículo |
| `GET` | `/api/vehicle/customer/{dni}` | Detalle de alquiler de un cliente |

Todos los endpoints requieren autenticación (ver sección 8).

La documentación Swagger está disponible en `/swagger` en entorno Development.

---

## 7. Infraestructura y persistencia

### Base de datos: PostgreSQL 17

La conexión se configura mediante la variable de entorno `ConnectionStrings__Default`.  
El esquema se crea automáticamente al arrancar mediante `EnsureCreatedAsync()` (no usa migraciones EF).

### Esquema

```
vehicles
  vehicle_id       BIGSERIAL PK
  brand            TEXT NOT NULL
  model            TEXT NOT NULL
  license_plate    TEXT NOT NULL UNIQUE
  manufacture_year INT NOT NULL
  is_available     BOOLEAN NOT NULL

customers
  customer_id    BIGSERIAL PK
  customer_name  TEXT NOT NULL
  customer_dni   TEXT NOT NULL UNIQUE

rentals
  rental_id          UUID PK
  vehicle_id         BIGINT FK → vehicles (RESTRICT)
  customer_id        BIGINT FK → customers (RESTRICT)
  start_date         TIMESTAMP NOT NULL
  planned_end_date   TIMESTAMP NOT NULL
  returned_date      TIMESTAMP NULL
  INDEX (vehicle_id, returned_date)
  INDEX (customer_id)
```

### Unit of Work

`IUnitOfWork` proporciona control transaccional explícito. Los repositorios **no gestionan transacciones**; los casos de uso las abren, operan y hacen commit/rollback.

```
BeginTransactionAsync() → [operaciones de repositorio] → CommitAsync() / RollbackAsync()
```

---

## 8. Autenticación

### Development
API Key simple mediante la cabecera `X-Api-Key`.  
El valor se configura en `ApiKey__Value` (por defecto `x-api-key` en Docker).

```http
X-Api-Key: x-api-key
```

Los intentos fallidos (cabecera ausente o clave inválida) se registran como `Warning` con la IP remota.

### Production
JWT Bearer via **IdentityServer4**. La URL de autoridad se configura en `AppSettings:JwtAuthority`.

---

## 9. Observabilidad

El stack de observabilidad corre íntegramente en Docker.

### Trazas distribuidas — `System.Diagnostics.ActivitySource`

Todos los casos de uso crean actividades bajo el `ActivitySource` compartido `"GtMotive.Estimate.Renting"`:

| Actividad | Tags |
|-----------|------|
| `vehicle.add` | brand, model, license_plate, manufacture_year, vehicle_id |
| `vehicle.rent` | license_plate, customer_name, start/end dates, rental_id, customer_id |
| `vehicle.return` | license_plate, rental_id, customer_id, returned_date |

### Métricas — Prometheus + Grafana

- Prometheus recoge métricas del endpoint `/metrics` de la API.
- Grafana (`:3000`) visualiza dashboards provisonados automáticamente.
- Credenciales Grafana por defecto: `admin / admin`.

### Logs — Serilog + Loki

En Development, los logs de los casos de uso (nivel `Information` en adelante) y todos los `Error` se envían a **Loki** (`:3100`), visible desde Grafana.

### Telemetría de negocio — `ITelemetry`

Cada caso de uso emite eventos de negocio (`VehicleAdded`, `VehicleRented`, `VehicleReturned`) con propiedades estructuradas. En producción se redirigen a **Application Insights**.

---

## 10. Cómo ejecutar

### Requisitos
- Docker Desktop

### Arrancar el stack completo

```bash
docker compose up --build -d
```

Servicios disponibles:

| Servicio | URL |
|----------|-----|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 |
| Loki | http://localhost:3100 |
| PostgreSQL | localhost:5432 |

### Ejemplo de uso rápido

```bash
# 1. Añadir vehículo
curl -X POST http://localhost:8080/api/vehicle \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: x-api-key" \
  -d '{"brand":"Toyota","model":"Corolla","licensePlate":"1234ABC","manufactureYear":2022}'

# 2. Alquilar vehículo
curl -X POST http://localhost:8080/api/vehicle/1234ABC/rent \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: x-api-key" \
  -d '{"customerName":"Juan García","customerDni":"12345678A","startDate":"2026-05-01","plannedEndDate":"2026-05-07"}'

# 3. Devolver vehículo
curl -X POST http://localhost:8080/api/vehicle/1234ABC/return \
  -H "X-Api-Key: x-api-key"
```

### Reset completo de base de datos

```bash
docker compose down -v && docker compose up --build -d
```

---

## 11. Tests

### Tests unitarios (`test/unit/`)

Cubren los casos de uso de forma aislada con **xUnit** y **Moq**.  
Se mockean todos los repositorios y el output port.

```bash
dotnet test test/unit/
```

Casos cubiertos:
- `AddVehicleUseCase`: vehículo añadido correctamente, año inválido (>5 años), matrícula duplicada.
- `RentVehicleUseCase`: alquiler correcto, vehículo no encontrado, vehículo no disponible, cliente ya con alquiler, fechas inválidas.
- `ReturnVehicleUseCase`: devolución correcta, vehículo no encontrado, sin alquiler activo.

### Tests de integración (`test/infrastructure/`)

Arrancan un servidor real en memoria con **Acheve.TestHost** y base de datos en memoria EF Core.  
Verifican el pipeline HTTP completo (routing, autenticación, filtros, presenters).

```bash
dotnet test test/infrastructure/
```
