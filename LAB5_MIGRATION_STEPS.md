# Lab 5 Migration Steps

After changing the model, apply the database update with Entity Framework Core:

```bash
dotnet ef migrations add AddIdentityAndAttachments
dotnet ef database update
```

If you need to recreate the local database from scratch:

```bash
dotnet ef database drop --force
dotnet ef database update
```

For this project, startup now uses `Database.Migrate()` so the schema is applied automatically when the app starts and the connection string points to the SQL Server database.
