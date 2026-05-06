# Runtime Configuration Matrix

| Variable | Local | Staging | Production | Fuente |
| --- | --- | --- | --- | --- |
| ASPNETCORE_ENVIRONMENT | Development | Staging | Production | Environment |
| DOTNET_ENVIRONMENT | Development | Staging | Production | Environment |
| ConnectionStrings__SqlServer | LocalDB/SQL container | Secret | Secret | Secret manager |
| Authentication__Mode | Development | JwtBearer | JwtBearer | Environment |
| Authentication__Authority | vacío | Secret/config | Secret/config | Identity provider |
| Authentication__Audience | vacío | Secret/config | Secret/config | Identity provider |
| Cors__AllowedOrigins__0 | localhost | HTTPS staging domain | HTTPS production domain | Environment |
| AllowedHosts | localhost | staging host | production host | Environment |
| Security__RequireHttps | true/false según host local | true | true | Environment |
| Security__RateLimiting__Enabled | false para smoke local | true | true | Environment |

## Regla

Staging y Production no deben usar Development auth, LocalDB, localhost SQL Server ni secretos en archivos.
