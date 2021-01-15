#!/bin/bash

ROOT_DIR=$(pwd) # Be sure to run this from root directory!
APP_NAME="${ROOT_DIR}/dmg-source/Tippy.app"
PUBLISH_OUTPUT_DIRECTORY="${ROOT_DIR}/tippy-osx-x64"

echo "Dotnet publishing..."
dotnet publish src/Tippy/Tippy.csproj -c Release -r osx-x64 --self-contained true -o $PUBLISH_OUTPUT_DIRECTORY

echo "Creating app..."
test -f "$APP_NAME" && rm -rf "$APP_NAME"
mkdir -p "$APP_NAME"
mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "${ROOT_DIR}/tools/osx/Info.plist" "$APP_NAME/Contents/Info.plist"
cp "${ROOT_DIR}/tools/osx/Tippy.entitlements" "$APP_NAME/Contents/Tippy.entitlements"
cp "${ROOT_DIR}/tools/osx/Tippy.icns" "$APP_NAME/Contents/Resources/Tippy.icns"
cp -a "$PUBLISH_OUTPUT_DIRECTORY/" "$APP_NAME/Contents/MacOS"
cp "${ROOT_DIR}/tools/osx/main" "$APP_NAME/Contents/MacOS"
chmod +x "$APP_NAME/Contents/MacOS/main"
cp "${ROOT_DIR}/tools/osx/console" "$APP_NAME/Contents/MacOS"
chmod +x "$APP_NAME/Contents/MacOS/console"
