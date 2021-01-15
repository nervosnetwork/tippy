#!/bin/bash

# Sign and create dmg

echo "$CERTIFICATE" | base64 --decode > certificate.p12
security create-keychain -p "$KEYCHAIN_PASSWORD" sign.keychain
security default-keychain -s sign.keychain
security unlock-keychain -p "$KEYCHAIN_PASSWORD" sign.keychain
security import certificate.p12 -k sign.keychain -P "$CERTIFICATE_PASSWORD" -T /usr/bin/codesign
security set-key-partition-list -S apple-tool:,apple:,codesign: -s -k "$KEYCHAIN_PASSWORD" sign.keychain
SIGN_IDENTITY=$(security find-identity -v -p codesigning sign.keychain | head -1 | grep '"' | sed -e 's/[^"]*"//' -e 's/".*//')
echo "Sign identity: $SIGN_IDENTITY"
SIGN_OPTIONS="--force --deep --options runtime --timestamp --entitlements ./tools/osx/Tippy.entitlements -v"
/usr/bin/codesign -s "$SIGN_IDENTITY" $SIGN_OPTIONS Tippy.app
mkdir dmg-source
mv Tippy.app dmg-source/
create-dmg --volname "Tippy" --window-size 600 400 --icon-size 100 Tippy.dmg dmg-source/
/usr/bin/codesign -s "$SIGN_IDENTITY" $SIGN_OPTIONS Tippy.dmg
