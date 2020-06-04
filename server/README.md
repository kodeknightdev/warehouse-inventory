# Warehouse Pulse - API

ASP.NET Web API 2 on .NET Framework 4.8, Entity Framework 6 (code first migrations), SQL Server.

## Setup

1. Open `WarehousePulse.sln` in Visual Studio 2019, packages restore on build.
2. The connection string in `Web.config` points at LocalDB (`(LocalDb)\MSSQLLocalDB`, database `WarehousePulse`).
   Change it if you use a full SQL Server instance.
3. Package Manager Console, default project `WarehousePulse.Api`:

   ```
   Update-Database
   ```

   This applies all migrations and runs the seed (a few locations and items).
4. F5 - runs on IIS Express at `https://localhost:44358`. Swagger UI is at `/swagger`.

Adding a migration after changing the model:

```
Add-Migration SomeName
Update-Database
```

## Settings (appSettings in Web.config)

| Key | Default | |
| --- | --- | --- |
| `CorsOrigins` | `http://localhost:4200` | comma separated |
| `LowStockMonitorEnabled` | `true` | background low stock check |
| `LowStockCheckMinutes` | `5` | how often it runs |

## Low stock alerts

`Background/LowStockMonitor` runs on a timer inside the app pool (registered with `HostingEnvironment` so it
stops cleanly on recycle). Each run raises an alert for items at or below their reorder level that don't
already have an open alert, and resolves alerts for items that have been restocked.

Note: it only runs while the app pool is alive. If IIS idles the site out, nothing is checked until the next
request - set the app pool to AlwaysRunning if that matters.

## Tests

`WarehousePulse.Tests` (MSTest + Moq), run from Test Explorer.
