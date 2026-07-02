# NitroHttp Desktop

NitroHttp Desktop is the main Avalonia application for fast API testing. It focuses on a clean UI and practical workflows for sending requests, inspecting responses, and keeping recent work available locally.

## What it does

- Send requests quickly with `GET`, `POST`, `PUT`, `PATCH`, and `DELETE`
- Build query parameters visually
- Inspect response body, status, time, size, headers, and cookies
- Persist recent requests in history and reusable requests in collections

## Features

### Request builder

- HTTP methods: `GET`, `POST`, `PUT`, `PATCH`, `DELETE`
- URL input with automatic query-string sync from key/value parameter rows
- JSON request body editor powered by AvaloniaEdit

### Response viewer

- Status code plus reason phrase, for example `200 OK`
- Response count with object and array awareness for JSON
- Elapsed time in milliseconds and payload size in `B`, `KB`, or `MB`
- Response tabs for **Body**, **Headers**, and **Cookies**

### Request persistence

- History stores up to 50 unique recent requests
- Collections lets you save named requests
- Data is stored as JSON on disk and restored at app startup

## Tech stack

- .NET `net10.0`
- Avalonia `11.3.x`
- AvaloniaEdit `11.3.x`

## Project structure

- `Program.cs` - desktop app entrypoint
- `App.axaml`, `App.axaml.cs` - application setup and theme wiring
- `MainWindow.axaml` - full UI layout
- `MainWindow.axaml.cs` - request execution, tab logic, persistence, and response rendering
- `Helpers/HttpStatusHelper.cs` - status code text and status group helpers
- `Helpers/FormatBytes.cs` - human-readable response size formatting
- `Helpers/RequestStore.cs` - persisted model for `History` and `Collections`

## Prerequisites

- .NET SDK that supports `net10.0`
  - Verify with `dotnet --info`
- Linux, macOS, or Windows desktop environment

For Debian packaging:

- `dpkg-deb`
- `python3` and `pip`
- Pillow (`PIL`), which is auto-installed by the packaging script if missing

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

### Scripted publish

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

1. Select a method and enter a URL.
2. Add query params from the **Params** tab using `+ Add`.
3. Add JSON body content in the **Body** tab for write methods.
4. Click **NITRO** to send.
5. Inspect response stats and switch between **Body**, **Headers**, and **Cookies**.
6. Save frequently used requests in **Collections**.

## Current behavior and limitations

- Request body content type is JSON (`application/json`) for `POST`, `PUT`, and `PATCH`.
- Query parameters are applied by rewriting the URL from the Params panel.
- Response headers and cookies are currently populated for `GET` flow.
- **Headers** and **Auth** request input panels are present in the UI but are not yet applied to outgoing requests.
- History deduplicates by `Method`, `Url`, and `Body`, and keeps newest first.

## Troubleshooting

- **Blank response or URL error**: ensure the URL is not empty and includes `http://` or `https://`.
- **JSON not formatted**: the response is likely not valid JSON; NitroHttp shows raw text.
- **Packaging fails on Linux**: verify `dpkg-deb`, `python3`, and `pip` are installed.

## Roadmap ideas

- Apply custom request headers from the Headers tab
- Apply bearer or API-key auth from the Auth tab
- Export and import collections
- Save and copy response actions wiring
- Add more body types such as form-data and `x-www-form-urlencoded`
