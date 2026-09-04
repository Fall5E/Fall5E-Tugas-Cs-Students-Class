# 10,000 HOURS MVP

A fresh MVP foundation for the 10,000 Hours system blueprint.

## Stack
- ASP.NET Core Razor Pages (.NET 10)
- ASP.NET Core Identity (email/password login)
- Entity Framework Core + SQLite
- Cloud-media-ready references (`MediaItem` stores metadata/URLs/YouTube IDs)

## Core Modules
- **Login/Register**: identity and account-based data isolation.
- **Today**: single daily entry per user/day.
- **Timeline**: chronological life log view.
- **Archive/Search**: keyword search over journal/work/learning/context/activity/media refs.
- **10,000 Hours**: automatic activity-hour aggregation.
- **Media**: grouped listing for Photo / FaceOfDay / Documentary references.

## Data Model
- `Goals`
- `DailyEntries` (unique by `UserId + EntryDate`)
- `ActivityRecords`
- `MediaItems`

## Run
```bash
dotnet restore
dotnet ef database update
dotnet run
```

Open the app URL, create an account, then start writing from **Today**.
