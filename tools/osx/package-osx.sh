#!/bin/bash

ROOT_DIR=$(pwd) # Be sure to run this from root directory!
APP_NAME="${ROOT_DIR}/dmg-source/Tippy.app"
PUBLISH_OUTPUT_DIRECTORY="${ROOT_DIR}/tippy-osx-x64"
KEYCHAIN_PASSWORD="whatever"

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

# $CERTIFICATE: base64 certificates.p12
echo $CERTIFICATE | base64 —decode > certificate.p12
security create-keychain -p $KEYCHAIN_PASSWORD sign.keychain
security default-keychain -s sign.keychain
security unlock-keychain -p $KEYCHAIN_PASSWORD sign.keychain
security import certificate.p12 -k sign.keychain -P $CERTIFICATE_PASSWORD -T /usr/bin/codesign
security set-key-partition-list -S apple-tool:,apple:,codesign: -s -k $KEYCHAIN_PASSWORD sign.keychain
CERTIFICATE_IDENTITY=$(security find-identity -v -p codesigning sign.keychain | head -1 | grep '"' | sed -e 's/[^"]*"//' -e 's/".*//')

SIGN_OPTIONS = "--force -s $CERTIFICATE_IDENTITY --deep --options runtime --timestamp --entitlements ${ROOT_DIR}/tools/osx/Tippy.entitlementsi -v"
echo "Signing app with identity $CERTIFICATE_IDENTITY..."
/usr/bin/codesign $SIGN_OPTIONS $APP_NAME

echo "Creating dmg..."
test -f Tippy.dmg && rm Tippy.dmg
# brew install create-dmg
create-dmg \
  --volname "Tippy" \
  --volicon "$ICON_FILES" \
  --window-size 600 400 \
  --icon-size 100 \
  "Tippy.dmg" \
  "${ROOT_DIR}/dmg-source/"

echo "Signing dmg..."
/usr/bin/codesign $SIGN_OPTIONS Tippy.dmg

echo "All done."