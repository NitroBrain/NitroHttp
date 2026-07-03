# NitroHttp CLI Build and Publish

For maximum performance, publish a release build for the target runtime. This project is configured for NativeAOT, so `dotnet publish` produces an ahead-of-time compiled binary for the supported runtime.

## Linux

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

## Windows

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## Notes

- The CLI project enables `PublishAot=true`, so publish output is AOT-compiled.
- The publish scripts use single-file, self-contained outputs for easier distribution.