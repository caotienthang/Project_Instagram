
# Project_Instagram (WinForms)

Small Windows Forms application demonstrating an Instagram OAuth login flow, account/session storage and a simple dashboard.

## Requirements

- Windows
- .NET Framework 4.8
- Visual Studio (recommended)
- WebView2 runtime (if WebView2 is used for OAuth)

## Quick start

1. Restore NuGet packages in Visual Studio.
2. Configure `config.json` with your Instagram app values (see Config section).
3. Build and run the solution.

Note: the app stores a local SQLite file at `Application.StartupPath\\SQLite\\data.db`.

## Important: SQLite provider initialization

If you get an exception saying "You need to call SQLitePCL.raw.SetProvider()...", initialize the SQLitePCL provider before any database access. Two options:

- Recommended: add a bundle package and call Batteries.Init():
  1. Install NuGet package `SQLitePCLRaw.bundle_e_sqlite3` (or `SQLitePCLRaw.bundle_winsqlite3`).
  2. In `Program.Main()` before `SqliteHelper.EnsureDatabase()` add:
     `SQLitePCL.Batteries.Init();`

- Alternative (manual): call `SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());` early in startup.

This project expects the provider to be initialized before `SqliteHelper.CreateTables()` opens the database.

## Configuration

- `config.json`: put Instagram `client_id`, `client_secret` (avoid committing secrets), and `redirect_uri`.

Example keys:

```
{
  "clientId": "YOUR_CLIENT_ID",
  "clientSecret": "YOUR_CLIENT_SECRET",
  "redirectUri": "https://localhost/"
}
```

## Project structure (high level)

- `Forms/` - WinForms UI: `AccountsCenterForm`, `DashBoard`, `AccountDetailForm`, etc.
- `Views/` - UserControls used by the dashboard.
- `Services/` - API and business logic (Instagram auth, posts, account services).
- `Data/` - Repositories for SQLite storage (account/session persistence).
- `Models/` - Domain models (AccountInfo, InstagramSession, AuthResult).
- `Helpers/SqliteHelper.cs` - helper that creates `data.db` and the tables; uses `Microsoft.Data.Sqlite`.

## OAuth flow (summary)

1. Open WebView2 and navigate to Instagram OAuth URL.
2. User signs in and Instagram redirects to `redirect_uri` with `?code=...`.
3. Exchange `code` for an access token via Instagram API.
4. Load basic user info and save account/session locally.

## Troubleshooting

- "You need to call SQLitePCL.raw.SetProvider()...": see "Important: SQLite provider initialization" above.
- "Invalid redirect_uri": confirm `redirect_uri` in Instagram app settings matches `config.json`.
- If WebView2 does not load, install the WebView2 runtime.

## Contributing

Open a pull request for bug fixes or improvements. Keep secrets out of the repository.

## License

This repository follows the license in the project root (if any). If none, assume permissive use only with attribution.

