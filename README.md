<img width="9294" height="2010" alt="Frame@2x" src="https://github.com/user-attachments/assets/3684cad3-3c2c-4787-abf4-90d0a7e3310b" />

<h3 align="center">A modern, minimal lightweight API testing tool</h3>

<img width="2072" height="1193" alt="image" src="https://github.com/user-attachments/assets/d5ff846e-7019-4c2a-b084-1eff6f70f188" />

---

## NitroHttp

NitroHttp is a desktop HTTP client for fast API testing.

It focuses on a clean UI and practical workflows:
- Send requests quickly (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`)
- Build query parameters visually
- Inspect response body, status, time, size, headers, and cookies
- Persist recent requests (history) and reusable requests (collections)

## Features

### Request builder
- HTTP methods: `GET`, `POST`, `PUT`, `PATCH`, `DELETE`
- URL input with automatic query-string sync from key/value parameter rows
- JSON request body editor (AvaloniaEdit)

### Response viewer
- Status code + reason phrase (e.g., `200 OK`)
- Response count (object/array aware for JSON)
- Elapsed time (`ms`) and payload size (`B`, `KB`, `MB`)
- Response tabs:
	- **Body** (pretty-printed JSON when possible)
	- **Headers**
	- **Cookies**

### Request persistence
- **History** stores up to 50 unique recent requests
- **Collections** lets you save named requests
- Data is stored as JSON on disk and restored at app startup

## Tech stack

- .NET `net10.0`
- Avalonia `11.3.x`
- AvaloniaEdit `11.3.x`

## Project structure

- `Program.cs` – desktop app entrypoint
- `App.axaml`, `App.axaml.cs` – application setup and theme wiring
- `MainWindow.axaml` – full UI layout
- `MainWindow.axaml.cs` – request execution, tab logic, persistence, response rendering
- `Helpers/HttpStatusHelper.cs` – status code text + status group helpers
- `Helpers/FormatBytes.cs` – human-readable response size formatting
- `Helpers/RequestStore.cs` – persisted model (`History`, `Collections`)
- `scripts/publish-all.sh` – multi-runtime publish helper
- `scripts/package-deb.sh` – Linux `.deb` packaging helper

## Prerequisites

- .NET SDK that supports `net10.0`
	- Verify with: `dotnet --info`
- Linux/macOS/Windows desktop environment

For Debian packaging:
- `dpkg-deb`
- `python3` + `pip`
- Pillow (`PIL`) (auto-installed by script if missing)

## Run locally

```bash
dotnet restore
dotnet run
```

## Build

```bash
dotnet build -c Release
```

## Publish binaries

### Scripted publish (Linux + Windows)

```bash
bash scripts/publish-all.sh
```

Outputs:
- `publish/linux-x64/`
- `publish/win-x64/`

### Manual publish example

```bash
dotnet publish NitroHttp.csproj \
	-c Release \
	-r linux-x64 \
	--self-contained true \
	/p:PublishSingleFile=true \
	/p:IncludeNativeLibrariesForSelfExtract=true \
	/p:PublishTrimmed=false \
	-o publish/linux-x64
```

## Package as Debian `.deb`

```bash
bash scripts/package-deb.sh 1.0.0
```

Output package:
- `dist/nitrohttp_1.0.0_amd64.deb`

Install:

```bash
sudo dpkg -i dist/nitrohttp_1.0.0_amd64.deb
```

## Data storage

NitroHttp stores request history and collections in a JSON file:

- Path base: `Environment.SpecialFolder.ApplicationData`
- Relative file: `NitroHttp/requests.json`

On Linux this typically resolves to:
- `~/.config/NitroHttp/requests.json`

Stored model:
- `History[]`
- `Collections[]`
- Each item includes `Method`, `Url`, `Body`, `Timestamp`, and optional `CollectionName`

## Usage guide

1. Select method and enter URL.
2. Add query params from the **Params** tab (`+ Add`).
3. Add JSON body in **Body** tab for write methods.
4. Click **NITRO** to send.
5. Inspect response stats and switch between **Body / Headers / Cookies** tabs.
6. Save frequently used requests in **Collections**.

## Current behavior and limitations

- Request body content type is JSON (`application/json`) for `POST`, `PUT`, `PATCH`.
- Query parameters are applied by rewriting the URL from the Params panel.
- Response headers and cookies are currently populated for `GET` flow.
- **Headers** and **Auth** request input panels are present in UI but not yet applied to outgoing requests.
- History deduplicates by (`Method`, `Url`, `Body`) and keeps newest first.

## Troubleshooting

- **Blank response or URL error**: ensure URL is not empty and includes `http://` or `https://`.
- **JSON not formatted**: response is likely not valid JSON; NitroHttp shows raw text.
- **Packaging fails on Linux**: verify `dpkg-deb`, `python3`, and `pip` are installed.

## Roadmap ideas

- Apply custom request headers from the Headers tab
- Apply bearer/API-key auth from the Auth tab
- Export/import collections
- Save/copy response actions wiring
- Additional body types (form-data, x-www-form-urlencoded)
