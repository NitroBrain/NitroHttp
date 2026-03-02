#!/usr/bin/env bash
set -euo pipefail

PROJECT="NitroHttp.csproj"
CONFIGURATION="Release"
RUNTIME="linux-x64"
APP_NAME="NitroHttp"
PACKAGE_NAME="nitrohttp"
ARCH="amd64"
VERSION="${1:-1.0.0}"
ICON_SOURCE="Assets/app.ico"

PUBLISH_DIR="publish/${RUNTIME}"
DIST_DIR="dist"
ICON_PNG_TMP="${DIST_DIR}/${PACKAGE_NAME}.png"
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
mkdir -p "$PKG_ROOT/DEBIAN" "$PKG_ROOT/usr/bin" "$PKG_ROOT/usr/share/applications" "$PKG_ROOT/usr/share/icons/hicolor/256x256/apps"

install -m 755 "$PUBLISH_DIR/${APP_NAME}" "$PKG_ROOT/usr/bin/${PACKAGE_NAME}"

python3 - <<PY
from PIL import Image

source = "${ICON_SOURCE}"
target = "${ICON_PNG_TMP}"

with Image.open(source) as image:
  image = image.convert("RGBA")
  image.save(target, format="PNG")
PY

install -m 644 "$ICON_PNG_TMP" "$PKG_ROOT/usr/share/icons/hicolor/256x256/apps/${PACKAGE_NAME}.png"
rm -f "$ICON_PNG_TMP"

cat > "$PKG_ROOT/DEBIAN/control" <<EOF
Package: ${PACKAGE_NAME}
Version: ${VERSION}
Section: net
Priority: optional
Architecture: ${ARCH}
Maintainer: NitroHttp <support@example.com>
Depends: libc6
Description: NitroHttp desktop HTTP client
EOF

cat > "$PKG_ROOT/usr/share/applications/${PACKAGE_NAME}.desktop" <<EOF
[Desktop Entry]
Name=${APP_NAME}
Comment=HTTP client
Exec=${PACKAGE_NAME}
Icon=${PACKAGE_NAME}
Terminal=false
Type=Application
Categories=Network;Utility;
EOF

cat > "$PKG_ROOT/DEBIAN/postinst" <<'EOF'
#!/usr/bin/env bash
set -e

if command -v update-desktop-database >/dev/null 2>&1; then
  update-desktop-database /usr/share/applications || true
fi

if command -v gtk-update-icon-cache >/dev/null 2>&1; then
  gtk-update-icon-cache -f /usr/share/icons/hicolor || true
fi
EOF
chmod 755 "$PKG_ROOT/DEBIAN/postinst"

dpkg-deb --build --root-owner-group "$PKG_ROOT" "$DEB_FILE"

echo "Created ${DEB_FILE}"
