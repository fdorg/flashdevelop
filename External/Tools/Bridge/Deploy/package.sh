#!/bin/bash

DIR="tmp"
APP="FlashDevelop Bridge.app"
DMG="FlashDevelop-Bridge.dmg"

PATH=/Users/Mika/QtSDK/5.3.2/5.3/clang_64/bin:$PATH
export PATH

echo Remove old files...
if [ ! -d "$DIR" ]; then
  mkdir "$DIR"
elif [ -d "$DIR/$APP" ]; then
  rm -rf "$DIR/$APP"
fi
if [ -e "$DMG" ]; then
  rm -rf "$DMG"
fi

echo Copy clean build...
cp -r "../Bin/Release/$APP" "$DIR/$APP"

echo Include Qt dependencies...
macdeployqt "$DIR/$APP"

echo Add FDEXE.sh...
cp "FDEXE.sh" "$DIR/$APP/Contents/MacOS/FDEXE.sh"
chmod +x "$DIR/$APP/Contents/MacOS/FDEXE.sh"

echo Package DMG...
hdiutil create -fs HFS+ -volname "$DMG" -srcfolder "$DIR" "$DMG"
