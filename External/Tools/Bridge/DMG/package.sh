#!/bin/bash
DIR="FlashDevelop Bridge"
APP="FlashDevelopBridge.app"
DMG="FlashDevelop Bridge"

echo Cleanup...
if [ ! -d "$DIR" ]; then
  mkdir "$DIR"
elif [ -d "$DIR/$APP" ]; then
  rm -rf "$DIR/$APP"
fi

if [ -e "$DMG.dmg" ]; then
  rm -rf "$DMG.dmg"
fi

cp -r "../../FlashDevelopBridge-build-desktop/$APP" "$DIR/$APP"

echo Include Qt dependencies...
macdeployqt "$DIR/$APP" -no-plugins

echo Package DMG
hdiutil create -fs HFS+ -volname "$DMG" -srcfolder "$DIR" "$DMG.dmg"
