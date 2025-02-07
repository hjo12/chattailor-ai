# ChatTailor.DataAccess

This project contains the data access layer for the ChatTailor project. A .NET 8 Class Library project that uses Entity Framework Core to interact with the database.

## Getting Started

### Running Migrations

```
dotnet ef migrations add Initial --context SQLiteDb --output-dir Database/Migrations/SQLite
```