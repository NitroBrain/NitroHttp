#!/usr/bin/env bash
set -euo pipefail

PROJECT="Nitrohttp.csproj"
CONFIGURATION="Release"
RUNTIME="linux-x64"
APP_NAME="Nitrohttp"
PACKAGE_NAME="nitrohttp"
ARCH="amd64"
VERSION="${1:-1.0.0}"

PUBLISH_DIR="publish/${RUNTIME}"
DIST_DIR="dist"
PKG_ROOT="${DIST_DIR}/${PACKAGE_NAME}_${VERSION}_${ARCH}"
DEB_FILE="${DIST_DIR}/${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"

echo "Publishing ${APP_NAME} for ${RUNTIME}..."
dotnet publish "$PROJECT" \
  -c "$CONFIGURATION" \
  -r "$RUNTIME" \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true \
  /p:PublishTrimmed=false \
  -o "$PUBLISH_DIR"

rm -rf "$PKG_ROOT"
mkdir -p "$PKG_ROOT/DEBIAN" "$PKG_ROOT/usr/bin" "$PKG_ROOT/usr/share/applications"

install -m 755 "$PUBLISH_DIR/${APP_NAME}" "$PKG_ROOT/usr/bin/${PACKAGE_NAME}"

cat > "$PKG_ROOT/DEBIAN/control" <<EOF
Package: ${PACKAGE_NAME}
Version: ${VERSION}
Section: net
Priority: optional
Architecture: ${ARCH}
Maintainer: Nitrohttp <support@example.com>
Depends: libc6
Description: Nitrohttp desktop HTTP client
EOF

cat > "$PKG_ROOT/usr/share/applications/${PACKAGE_NAME}.desktop" <<EOF
[Desktop Entry]
Name=${APP_NAME}
Comment=HTTP client
Exec=${PACKAGE_NAME}
Terminal=false
Type=Application
Categories=Network;Utility;
EOF

dpkg-deb --build --root-owner-group "$PKG_ROOT" "$DEB_FILE"

echo "Created ${DEB_FILE}"
