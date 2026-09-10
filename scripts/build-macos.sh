#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
BUILD_DIR="${BUILD_DIR:-build}"
mkdir -p "$BUILD_DIR/module-cache" "$BUILD_DIR/Komorebi.app/Contents/MacOS" "$BUILD_DIR/Komorebi.app/Contents/Resources/web" "$BUILD_DIR/Komorebi.app/Contents/Resources/licenses"
for arch in arm64 x86_64; do
  swiftc macos/main.swift -O -target "${arch}-apple-macosx12.0" -module-cache-path "$BUILD_DIR/module-cache" -framework Cocoa -framework WebKit -o "$BUILD_DIR/komorebi-$arch"
done
lipo -create "$BUILD_DIR/komorebi-arm64" "$BUILD_DIR/komorebi-x86_64" -output "$BUILD_DIR/Komorebi.app/Contents/MacOS/Komorebi"
cp web/index.html "$BUILD_DIR/Komorebi.app/Contents/Resources/web/"
cp licenses/* "$BUILD_DIR/Komorebi.app/Contents/Resources/licenses/"
cp macos/Info.plist "$BUILD_DIR/Komorebi.app/Contents/Info.plist"
codesign --force --sign - "$BUILD_DIR/Komorebi.app"
mkdir -p "$BUILD_DIR/dmg"
ditto "$BUILD_DIR/Komorebi.app" "$BUILD_DIR/dmg/Komorebi.app"
ln -sfn /Applications "$BUILD_DIR/dmg/Applications"
cp INSTALL-macOS.txt "$BUILD_DIR/dmg/READ-ME-FIRST.txt"
hdiutil create -volname 'Komorebi' -srcfolder "$BUILD_DIR/dmg" -ov -format UDZO "$BUILD_DIR/Komorebi-1.1.0-macOS-universal.dmg"
hdiutil verify "$BUILD_DIR/Komorebi-1.1.0-macOS-universal.dmg"
codesign --verify --deep --strict "$BUILD_DIR/Komorebi.app"
lipo "$BUILD_DIR/Komorebi.app/Contents/MacOS/Komorebi" -verify_arch arm64 x86_64
