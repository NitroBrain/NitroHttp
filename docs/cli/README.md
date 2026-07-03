# NitroHttp CLI

NitroHttp CLI is the command-line companion to nhttp. It is a blazing fast HTTP testing tool for quick API checks from a terminal, with formatted JSON output and response statistics.

## Overview

The CLI exposes five subcommands:

- `get`
- `post`
- `put`
- `patch`
- `delete`

Short aliases are also available:

- `g` for `get`
- `p` for `post`
- `pu` for `put`
- `pa` for `patch`
- `del` for `delete`

## General usage

```bash
nhttp <command> --url <endpoint> [--body <json-or-file>]
```

Rules to keep in mind:

- `--url` is required for every command.
- If the URL does not start with `http`, the CLI prefixes it with `https://`.
- `--body` is required for `post`, `put`, and `patch`.
- If the `--body` value points to an existing file, the CLI reads the file contents and sends that content instead of the raw path string.
- Request bodies are sent as JSON (`application/json`).

## Commands

### GET

Send a request and render the response body as formatted JSON when possible.

```bash
nhttp get --url https://jsonplaceholder.typicode.com/users
nhttp g --url jsonplaceholder.typicode.com/users
```

Behavior:

- The response body is displayed in a bordered JSON table.
- The stats panel shows response status, elapsed time, payload size, and item count.
- If the JSON response is an array, the item count matches the array length.
- If the JSON response is a single object, the item count is `1`.

### POST

Create a resource or submit JSON to an endpoint.

```bash
nhttp post --url https://jsonplaceholder.typicode.com/users --body '{"name":"Ada"}'
nhttp p --url jsonplaceholder.typicode.com/users --body ./payloads/user.json
```

### PUT

Replace a resource with JSON content.

```bash
nhttp put --url https://jsonplaceholder.typicode.com/users/42 --body ./payloads/user-update.json
nhttp pu --url jsonplaceholder.typicode.com/users/42 --body '{"name":"Ada Lovelace"}'
```

### PATCH

Update part of a resource with JSON content.

```bash
nhttp patch --url https://jsonplaceholder.typicode.com/users/42 --body '{"title":"Engineer"}'
nhttp pa --url jsonplaceholder.typicode.com/users/42 --body ./payloads/user-patch.json
```

### DELETE

Remove a resource.

```bash
nhttp delete --url https://jsonplaceholder.typicode.com/users/42
nhttp del --url jsonplaceholder.typicode.com/users/42
```

## Output

Successful requests are rendered with:

- A formatted response body when the response can be parsed as JSON
- A response stats panel showing:
	- status text and HTTP code
	- elapsed time in milliseconds
	- response size in human-readable form
	- item count

If an error occurs, the CLI prints the message in a red error panel.

## Build and Publish

For AOT builds and runtime-specific publish commands, see [BUILD.md](BUILD.md).

## Examples

Fetch a collection:

```bash
nhttp get --url jsonplaceholder.typicode.com/posts
```

Create from inline JSON:

```bash
nhttp post --url https://jsonplaceholder.typicode.com/posts --body '{"title":"New post","body":"Hello"}'
```

Update from a file:

```bash
nhttp patch --url https://jsonplaceholder.typicode.com/posts/10 --body ./patch.json
```

Delete a record:

```bash
nhttp delete --url jsonplaceholder.typicode.com/posts/10
```
