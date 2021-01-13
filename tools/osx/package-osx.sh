#!/bin/bash

ROOT_DIR=$(pwd) # Be sure to run this from root directory!
APP_NAME="${ROOT_DIR}/Tippy.app"
PUBLISH_OUTPUT_DIRECTORY="${ROOT_DIR}/tippy-osx-x64"
INFO_PLIST="${ROOT_DIR}/tools/osx/Info.plist"
ICON_FILE="${ROOT_DIR}/tools/osx/Tippy.icns""

dotnet publish src/Tippy/Tippy.csproj -c Release -r osx-x64 --self-containe true -o $PUBLISH_OUTPUT_DIRECTORY

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"