#!/usr/bin/env bash
set -euo pipefail

PROJECT="NitroHttp.csproj"
CONFIGURATION="Release"

publish_target () {
  local runtime="$1"
  local output="publish/${runtime}"

  echo "Publishing ${runtime} -> ${output}"
  dotnet publish "$PROJECT" \
    -c "$CONFIGURATION" \
    -r "$runtime" \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:PublishTrimmed=false \
    -o "$output"
}

publish_target "linux-x64"
publish_target "win-x64"

echo "Done. Outputs: publish/linux-x64 and publish/win-x64"
