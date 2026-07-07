# User Cache API

This is a small ASP.NET Core Web API built for the assessment. It reads user data from the public JSONPlaceholder API and stores the results in SQL Server so the same data can be served from the database on later requests.

I used the JSONPlaceholder users endpoint because it is public, stable, and does not need an API key.

```text
https://jsonplaceholder.typicode.com/users
```

## What The API Does

The API exposes two endpoints:

```http
GET /api/users
GET /api/users/{id}
```

The first endpoint returns all users. The second endpoint returns a single user by ID.

Both endpoints check SQL Server first. If the requested data is already stored in the database, the API returns it from there. If it is not available yet, the API calls JSONPlaceholder, saves the result in SQL Server, and then returns it.

Database access is done with SQL queries using `Microsoft.Data.SqlClient`. No ORM is used.

## Running The Project

Open a terminal in the project folder

Restore the NuGet packages:

```powershell
dotnet restore UserCacheApi.csproj
```

Run the API:

```powershell
dotnet run --project UserCacheApi.csproj
```

The app should start on URLs similar to this:

```text
https://localhost:7222
```

## SQL Server Setup

The database schema is in this file:

```text
scripts/schema.sql
```

My local SQL setup uses LocalDB:

```text
(localdb)\MSSQLLocalDB
```

For that setup, the connection string in `appsettings.json` should be:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=UserCacheDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

To create the database and table, run this from the project folder:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i scripts/schema.sql -C
```

The script creates:

```text
Database: UserCacheDb
Table: dbo.Users
```

## Testing The API

You can test with curl:

```powershell
curl http://localhost:7222/api/users/1
curl http://localhost:7222/api/users
```

Swagger is also available after the API starts:

```text
http://localhost:7222/swagger
```

or, if HTTPS is working on the machine:

```text
https://localhost:7222/swagger
```

Swagger is useful for quickly trying both endpoints without writing curl commands.

## Cache Flow

For `GET /api/users/{id}`:

1. Look for the user in `dbo.Users`.
2. Return the database record if it exists.
3. If it does not exist, call JSONPlaceholder API.
4. Save the user into SQL Server.
5. Return the saved user.

For `GET /api/users`:

1. Check whether any users already exist in `dbo.Users`.
2. Return the database records if they exist.
3. If the table is empty, fetch users from JSONPlaceholder API.
4. Save the users into SQL Server.
5. Return the saved users.
