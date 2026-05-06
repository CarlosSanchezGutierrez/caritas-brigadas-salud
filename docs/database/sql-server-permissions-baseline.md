# SQL Server Permissions Baseline

## Principio

Usar mínimo privilegio para todos los usuarios SQL Server.

## Usuario de aplicación

El usuario de aplicación es usado por la API runtime.

Debe:

- Tener permisos mínimos sobre tablas necesarias.
- Poder leer/escribir datos operativos según el diseño.
- No ser sysadmin.
- No ser db_owner.
- No crear, alterar ni eliminar tablas.
- No aplicar migraciones.

## Usuario de migraciones

El usuario de migraciones se usa solo en proceso controlado.

Debe:

- Aplicar cambios de esquema.
- Usarse solo desde pipeline o procedimiento aprobado.
- No estar configurado en runtime de la API.
- Rotarse y auditarse.

## Usuario de lectura/reporting

Si TI requiere reporting externo:

- Crear usuario separado.
- Solo lectura.
- Sin acceso a tablas sensibles salvo aprobación.

## db_owner

db_owner no debe usarse para la API runtime.

Si se requiere para migraciones, debe ser temporal, aprobado y auditado.

## Auditoría

TI debe poder identificar:

- Qué usuario aplicó migración.
- Cuándo se aplicó.
- Qué script se ejecutó.
- Qué versión de aplicación se desplegó.
