#!/bin/bash

xcrun altool --notarize-app -t osx -f Tippy.dmg --primary-bundle-id "com.nervos.tippy" -u "$APPLE_ID" -p "$APPLE_ID_PASSWORD" --output-format xml | tee notarize_result
request_id="$(cat notarize_result | grep -A1 "RequestUUID" | sed -n 's/\s*<string>\([^<]*\)<\/string>/\1/p' | xargs)"
echo "Notarization in progress, request id: $request_id"
echo "Waiting for approval..."
while true; do
    echo -n "."
    sleep 10 # We need to wait 10 sec, even for the first loop because Apple might still not have their own data...
    xcrun altool --notarization-info "$request_id" -u "$APPLE_ID" -p "$APPLE_ID_PASSWORD" > notarization_progress
    if grep -q "Status: success" notarization_progress; then
        echo ""
        cat notarization_progress
        echo "Notarization succeed"
        break
    elif grep -q "Status: in progress" notarization_progress; then
        continue
    else
        cat notarization_progress
        echo "Notarization failed"
        exit 1
    fi
done