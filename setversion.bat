@echo off
cd "%1"
git log --pretty=format:%h%d -n 1 | "External\Tools\SetVersion\SetVersion" "FlashDevelop\Properties\AssemblyInfo.cs"
