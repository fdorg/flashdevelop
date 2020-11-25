#!/bin/sh
target_dir=$1
cd $target_dir
mono External/Tools/SetVersion/SetVersion.exe FlashDevelop/Properties/AssemblyInfo.cs "%APPVEYOR_BUILD_NUMBER%"
