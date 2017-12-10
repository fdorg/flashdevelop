;--------------------------------

!include "MUI.nsh"
!include "Sections.nsh"
!include "FileAssoc.nsh"
!include "LogicLib.nsh"
!include "WordFunc.nsh"
!include "Config.nsh"

;--------------------------------

; Define version info
!define VERSION "5.3.1"

; Installer details
VIAddVersionKey "CompanyName" "${DIST_COMP}"
VIAddVersionKey "ProductName" "${DIST_NAME} Installer"
VIAddVersionKey "LegalCopyright" "${DIST_COPY}"
VIAddVersionKey "FileDescription" "${DIST_NAME} Installer"
VIAddVersionKey "ProductVersion" "${VERSION}.0"
VIAddVersionKey "FileVersion" "${VERSION}.0"
VIProductVersion "${VERSION}.0"

; The name of the installer
Name "${DIST_NAME}"

; The captions of the installer
Caption "${DIST_NAME} ${VERSION} Setup"
UninstallCaption "${DIST_NAME} ${VERSION} Uninstall"

; The file to write
OutFile "Binary\${DIST_NAME}.exe"

; Default installation folder
InstallDir "$PROGRAMFILES\${DIST_NAME}\"

; Define executable files
!define EXECUTABLE "$INSTDIR\${DIST_NAME}.exe"
!define WIN32RES "$INSTDIR\Tools\winres\winres.exe"
!define ASDOCGEN "$INSTDIR\Tools\asdocgen\ASDocGen.exe"

; Get installation folder from registry if available
InstallDirRegKey HKLM "Software\${DIST_NAME}" ""

; Vista redirects $SMPROGRAMS to all users without this
RequestExecutionLevel admin

; Use replace and version compare
!insertmacro WordReplace
!insertmacro VersionCompare

; Required props
SetFont /LANG=${LANG_ENGLISH} "Tahoma" 8
SetCompressor /SOLID lzma
CRCCheck on
XPStyle on

;--------------------------------

; Interface Configuration

!define MUI_HEADERIMAGE
!define MUI_ABORTWARNING
!define MUI_HEADERIMAGE_BITMAP "Graphics\Banner.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "Graphics\Wizard.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "Graphics\Wizard.bmp"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "See online guide to get started"
!define MUI_FINISHPAGE_SHOWREADME "${DIST_README}"

;--------------------------------

; Pages

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH
!insertmacro MUI_LANGUAGE "English"

;--------------------------------

; InstallTypes

InstType "Default"
InstType "Standalone/Portable"
InstType "un.Default"
InstType "un.Full"

;--------------------------------

; Functions

Function GetIsWine
	
	Push $0
	ClearErrors
	EnumRegKey $0 HKLM "SOFTWARE\Wine" 0
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function un.GetIsWine
	
	Push $0
	ClearErrors
	EnumRegKey $0 HKLM "SOFTWARE\Wine" 0
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function GetDotNETVersion
	
	Push $0
	ClearErrors
	ReadRegStr $0 HKLM "Software\Microsoft\NET Framework Setup\NDP\v4\Client" "Version"
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function GetFlashVersion
	
	Push $0
	ClearErrors
	ReadRegStr $0 HKLM "Software\Macromedia\FlashPlayer" "CurrentVersion"
	IfErrors 0 +5
	ClearErrors
	ReadRegStr $0 HKCU "Software\Macromedia\FlashPlayer" "FlashPlayerVersion"
	IfErrors 0 +2
	StrCpy $0 "not_found"
	${WordReplace} $0 "," "." "+" $1
	Exch $1
	
FunctionEnd

Function GetJavaVersion
	
	Push $0
	ClearErrors
	ReadRegStr $0 HKLM "Software\JavaSoft\Java Runtime Environment" "CurrentVersion"
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function GetFDVersion
	
	Push $0
	ClearErrors
	ReadRegStr $0 HKLM "Software\${DIST_NAME}" "CurrentVersion"
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function GetFDInstDir
	
	Push $0
	ClearErrors
	ReadRegStr $0 HKLM "Software\${DIST_NAME}" ""
	IfErrors 0 +2
	StrCpy $0 "not_found"
	Exch $0
	
FunctionEnd

Function NotifyInstall
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	SetOutPath "$INSTDIR"
	File "/oname=.update" "..\Bin\Debug\.local"
	User:
	SetOutPath "$LOCALAPPDATA\${DIST_NAME}"
	File "/oname=.update" "..\Bin\Debug\.local"
	Done:
	
FunctionEnd

Function GetNeedsReset
	
	Call GetFDVersion
	Pop $1
	Push $2
	${VersionCompare} $1 "5.0.0" $3
	${If} $1 == "not_found"
	StrCpy $2 "do_reset"
	${ElseIf} $3 == 2
	StrCpy $2 "do_reset"
	${Else}
	StrCpy $2 "is_ok"
	${EndIf}
	Exch $2
	
FunctionEnd

;--------------------------------

; Install Sections

Section "${DIST_NAME}" Main
	
	SectionIn 1 2 RO
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	
	; Clean library
	RMDir /r "$INSTDIR\Library"

	; Clean old Flex PMD
	IfFileExists "$INSTDIR\Tools\flexpmd\flex-pmd-command-line-1.1.jar" 0 +2
	RMDir /r "$INSTDIR\Tools\flexpmd"
	
	; Copy all files
	File /r /x .svn /x .empty /x *.db /x Exceptions.log /x .local /x .multi /x *.pdb /x *.vshost.exe /x *.vshost.exe.config /x *.vshost.exe.manifest /x "..\Bin\Debug\Data\" /x "..\Bin\Debug\Settings\" /x "..\Bin\Debug\Snippets\" /x "..\Bin\Debug\Templates\" "..\Bin\Debug\*.*"
	
	SetOverwrite off
	
	IfFileExists "$INSTDIR\.local" +6 0
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	RMDir /r "$INSTDIR\Projects"
	
	SetOutPath "$INSTDIR\Settings"
	File /r /x .svn /x .empty /x *.db /x LayoutData.fdl /x SessionData.fdb /x SettingData.fdb "..\Bin\Debug\Settings\*.*"
	
	SetOutPath "$INSTDIR\Snippets"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Snippets\*.*"
	
	SetOutPath "$INSTDIR\Templates"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Templates\*.*"

	SetOutPath "$INSTDIR\Projects"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Projects\*.*"

	; Remove PluginCore from plugins...
	Delete "$INSTDIR\Plugins\PluginCore.dll"

	; Remove ProjectManager from inst dir...
	Delete "$INSTDIR\ProjectManager.dll"
	
	; Patch CrossOver/Wine files, remove 64bit
	SetOverwrite on
	SetOutPath "$INSTDIR"
	Call GetIsWine
	Pop $0
	${If} $0 != "not_found"
	File /r /x .svn /x .empty /x *.db "CrossOver\*.*"
	Delete "$INSTDIR\*64.exe"
	Delete "$INSTDIR\*64.exe.config"
	Delete "$INSTDIR\*64.dll"
	${EndIf}
	
	; Write update flag file...
	Call NotifyInstall
	
SectionEnd

Section "Desktop Shortcut" DesktopShortcut
	
	SetOverwrite on
	SetShellVarContext all
	
	CreateShortCut "$DESKTOP\${DIST_NAME}.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	
SectionEnd

Section "Quick Launch Item" QuickShortcut
	
	SetOverwrite on
	SetShellVarContext all
	
	CreateShortCut "$QUICKLAUNCH\${DIST_NAME}.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	
SectionEnd

SectionGroup "Language" LanguageGroup

Section "No changes" NoChangesLocale
	
	; Don't change the locale
	
SectionEnd

Section "English" EnglishLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "en_US"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "en_US"
	FileClose $1
	Done:
	
SectionEnd

Section "Chinese" ChineseLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "zh_CN"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "zh_CN"
	FileClose $1
	Done:
	
SectionEnd

Section "Japanese" JapaneseLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "ja_JP"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "ja_JP"
	FileClose $1
	Done:
	
SectionEnd

Section "German" GermanLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "de_DE"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "de_DE"
	FileClose $1
	Done:
	
SectionEnd

Section "Basque" BasqueLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "eu_ES"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "eu_ES"
	FileClose $1
	Done:
	
SectionEnd


Section "Korean" KoreanLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\${DIST_NAME}\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "ko_KR"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\${DIST_NAME}\.locale" w
	IfErrors Done
	FileWrite $1 "ko_KR"
	FileClose $1
	Done:
	
SectionEnd

SectionGroupEnd

SectionGroup "Advanced"

Section "Start Menu Group" StartMenuGroup
	
	SectionIn 1	
	SetOverwrite on
	SetShellVarContext all
	
	CreateDirectory "$SMPROGRAMS\${DIST_NAME}"
	CreateShortCut "$SMPROGRAMS\${DIST_NAME}\${DIST_NAME}.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	WriteINIStr "$SMPROGRAMS\${DIST_NAME}\Documentation.url" "InternetShortcut" "URL" "${DIST_DOCS}"
	WriteINIStr "$SMPROGRAMS\${DIST_NAME}\Community.url" "InternetShortcut" "URL" "${DIST_COMMUNITY}"
	CreateShortCut "$SMPROGRAMS\${DIST_NAME}\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
	
SectionEnd

Section "Registry Modifications" RegistryMods
	
	SectionIn 1
	SetOverwrite on
	SetShellVarContext all
	
	Delete "$INSTDIR\.multi"
	Delete "$INSTDIR\.local"
	
	DeleteRegKey /ifempty HKCR "Applications\${DIST_NAME}.exe"	
	DeleteRegKey /ifempty HKLM "Software\Classes\Applications\${DIST_NAME}.exe"
	DeleteRegKey /ifempty HKCU "Software\Classes\Applications\${DIST_NAME}.exe"
	
	!insertmacro APP_ASSOCIATE "fdp" "${DIST_NAME}.Project" "${DIST_NAME} Project" "${WIN32RES},2" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdproj" "${DIST_NAME}.GenericProject" "${DIST_NAME} Generic Project" "${WIN32RES},2" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "hxproj" "${DIST_NAME}.HaXeProject" "${DIST_NAME} Haxe Project" "${WIN32RES},2" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "as2proj" "${DIST_NAME}.AS2Project" "${DIST_NAME} AS2 Project" "${WIN32RES},2" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "as3proj" "${DIST_NAME}.AS3Project" "${DIST_NAME} AS3 Project" "${WIN32RES},2" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "docproj" "${DIST_NAME}.DocProject" "${DIST_NAME} Docs Project" "${WIN32RES},2" "" "${ASDOCGEN}"
	!insertmacro APP_ASSOCIATE "lsproj" "${DIST_NAME}.LoomProject" "${DIST_NAME} Loom Project" "${WIN32RES},2" "" "${EXECUTABLE}"

	!insertmacro APP_ASSOCIATE "fdi" "${DIST_NAME}.Theme" "${DIST_NAME} Theme File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdm" "${DIST_NAME}.Macros" "${DIST_NAME} Macros File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdt" "${DIST_NAME}.Template" "${DIST_NAME} Template File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fda" "${DIST_NAME}.Arguments" "${DIST_NAME} Arguments File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fds" "${DIST_NAME}.Snippet" "${DIST_NAME} Snippet File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdb" "${DIST_NAME}.Binary" "${DIST_NAME} Binary File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdl" "${DIST_NAME}.Layout" "${DIST_NAME} Layout File" "${WIN32RES},1" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "fdz" "${DIST_NAME}.Zip" "${DIST_NAME} Zip File" "${WIN32RES},1" "" "${EXECUTABLE}"
	
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Project" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.GenericProject" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.HaXeProject" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.AS2Project" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.AS3Project" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.DocProject" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.LoomProject" "ShellNew"

	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Theme" "ShellNew"	
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Macros" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Template" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Arguments" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Snippet" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Binary" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Layout" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "${DIST_NAME}.Zip" "ShellNew"
	
	; Write uninstall section keys
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "InstallLocation" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "Publisher" "${DIST_COMP}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "DisplayVersion" "${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "DisplayName" "${DIST_NAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "Comments" "Thank you for using ${DIST_NAME}."
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "HelpLink" "${DIST_COMMUNITY}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "DisplayIcon" "${EXECUTABLE}"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}" "NoRepair" 1
	WriteRegStr HKLM "Software\${DIST_NAME}" "CurrentVersion" ${VERSION}
	WriteRegStr HKLM "Software\${DIST_NAME}" "" $INSTDIR
	WriteUninstaller "$INSTDIR\Uninstall.exe"

	; Optimize with NGEN, not on CrossOver
	;Call GetIsWine
	;Pop $0
	;${If} $0 == "not_found"
	;nsExec::ExecToLog /TIMEOUT=1000 '"$INSTDIR\FDOPT.cmd" install'
	;${EndIf}

	!insertmacro UPDATEFILEASSOC
	
SectionEnd

Section "Standalone/Portable" StandaloneMode
	
	SectionIn 2
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	File ..\Bin\Debug\.local
	
SectionEnd

Section "Multi Instance Mode" MultiInstanceMode
	
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	File ..\Bin\Debug\.multi
	
SectionEnd

SectionGroupEnd

;--------------------------------

; Install section strings

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${Main} "Installs the main program and other required files."
!insertmacro MUI_DESCRIPTION_TEXT ${RegistryMods} "Associates integral file types and adds the required uninstall configuration."
!insertmacro MUI_DESCRIPTION_TEXT ${StandaloneMode} "Runs as standalone using only local setting files. NOTE: Not for standard users and manual upgrade only."
!insertmacro MUI_DESCRIPTION_TEXT ${MultiInstanceMode} "Allows multiple instances of ${DIST_NAME} to be executed. NOTE: There are some open issues with this."
!insertmacro MUI_DESCRIPTION_TEXT ${NoChangesLocale} "Keeps the current language on update and defaults to English on clean install."
!insertmacro MUI_DESCRIPTION_TEXT ${EnglishLocale} "Changes ${DIST_NAME}'s display language to English on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${ChineseLocale} "Changes ${DIST_NAME}'s display language to Chinese on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${JapaneseLocale} "Changes ${DIST_NAME}'s display language to Japanese on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${GermanLocale} "Changes ${DIST_NAME}'s display language to German on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${BasqueLocale} "Changes ${DIST_NAME}'s display language to Basque on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${KoreanLocale} "Changes ${DIST_NAME}'s display language to Korean on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${StartMenuGroup} "Creates a start menu group and adds default ${DIST_NAME} links to the group."
!insertmacro MUI_DESCRIPTION_TEXT ${QuickShortcut} "Installs a ${DIST_NAME} shortcut to the Quick Launch bar."
!insertmacro MUI_DESCRIPTION_TEXT ${DesktopShortcut} "Installs a ${DIST_NAME} shortcut to the desktop."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------

; Uninstall Sections

Section "un.${DIST_NAME}" UninstMain
	
	SectionIn 1 2 RO
	SetShellVarContext all
	
	; Unoptimize with NGEN, not on CrossOver
	;Call un.GetIsWine
	;Pop $0
	;${If} $0 == "not_found"
	;nsExec::ExecToLog /TIMEOUT=1000 '"$INSTDIR\FDOPT.cmd" uninstall'
	;${EndIf}
	
	Delete "$DESKTOP\${DIST_NAME}.lnk"
	Delete "$QUICKLAUNCH\${DIST_NAME}.lnk"
	Delete "$SMPROGRAMS\${DIST_NAME}\${DIST_NAME}.lnk"
	Delete "$SMPROGRAMS\${DIST_NAME}\Documentation.url"
	Delete "$SMPROGRAMS\${DIST_NAME}\Community.url"
	Delete "$SMPROGRAMS\${DIST_NAME}\Uninstall.lnk"
	RMDir "$SMPROGRAMS\${DIST_NAME}"
	
	RMDir /r "$INSTDIR\Docs"
	RMDir /r "$INSTDIR\Library"
	RMDir /r "$INSTDIR\Plugins"
	RMDir /r "$INSTDIR\StartPage"
	RMDir /r "$INSTDIR\Projects"
	RMDir /r "$INSTDIR\Tools"
	
	IfFileExists "$INSTDIR\.local" +5 0
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	
	Delete "$INSTDIR\FDMT.cmd"
	Delete "$INSTDIR\FDOPT.cmd"
	Delete "$INSTDIR\README.txt"
	Delete "$INSTDIR\FirstRun.fdb"
	Delete "$INSTDIR\Exceptions.log"
	Delete "$INSTDIR\${DIST_NAME}.exe"
	Delete "$INSTDIR\${DIST_NAME}.exe.config"
	Delete "$INSTDIR\${DIST_NAME}64.exe"
	Delete "$INSTDIR\${DIST_NAME}64.exe.config"
	Delete "$INSTDIR\PluginCore.dll"
	Delete "$INSTDIR\SciLexer.dll"
	Delete "$INSTDIR\SciLexer64.dll"
	Delete "$INSTDIR\Scripting.dll"
	Delete "$INSTDIR\AStyle64.dll"
	Delete "$INSTDIR\AStyle.dll"
	Delete "$INSTDIR\Antlr3.dll"
	Delete "$INSTDIR\SwfOp.dll"
	Delete "$INSTDIR\Aga.dll"
	
	Delete "$INSTDIR\Uninstall.exe"
	RMDir "$INSTDIR"
	
	!insertmacro APP_UNASSOCIATE "fdp" "${DIST_NAME}.Project"
	!insertmacro APP_UNASSOCIATE "fdproj" "${DIST_NAME}.GenericProject"
	!insertmacro APP_UNASSOCIATE "hxproj" "${DIST_NAME}.HaXeProject"
	!insertmacro APP_UNASSOCIATE "as2proj" "${DIST_NAME}.AS2Project"
	!insertmacro APP_UNASSOCIATE "as3proj" "${DIST_NAME}.AS3Project"
	!insertmacro APP_UNASSOCIATE "docproj" "${DIST_NAME}.DocProject"
	!insertmacro APP_UNASSOCIATE "lsproj" "${DIST_NAME}.LoomProject"
	
	!insertmacro APP_UNASSOCIATE "fdi" "${DIST_NAME}.Theme"
	!insertmacro APP_UNASSOCIATE "fdm" "${DIST_NAME}.Macros"
	!insertmacro APP_UNASSOCIATE "fdt" "${DIST_NAME}.Template"
	!insertmacro APP_UNASSOCIATE "fda" "${DIST_NAME}.Arguments"
	!insertmacro APP_UNASSOCIATE "fds" "${DIST_NAME}.Snippet"
	!insertmacro APP_UNASSOCIATE "fdb" "${DIST_NAME}.Binary"
	!insertmacro APP_UNASSOCIATE "fdl" "${DIST_NAME}.Layout"
	!insertmacro APP_UNASSOCIATE "fdz" "${DIST_NAME}.Zip"
	
	DeleteRegKey /ifempty HKLM "Software\${DIST_NAME}"
	DeleteRegKey /ifempty HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${DIST_NAME}"
	
	DeleteRegKey /ifempty HKCR "Applications\${DIST_NAME}.exe"	
	DeleteRegKey /ifempty HKLM "Software\Classes\Applications\${DIST_NAME}.exe"
	DeleteRegKey /ifempty HKCU "Software\Classes\Applications\${DIST_NAME}.exe"

	!insertmacro UPDATEFILEASSOC
	
SectionEnd

Section /o "un.Settings" UninstSettings
	
	SectionIn 2
	
	Delete "$INSTDIR\.multi"
	Delete "$INSTDIR\.local"
	Delete "$INSTDIR\.locale"
	
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	RMDir /r "$LOCALAPPDATA\${DIST_NAME}"
	RMDir "$INSTDIR"
	
SectionEnd

;--------------------------------

; Uninstall section strings

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${UninstMain} "Uninstalls the main program, other required files and registry modifications."
!insertmacro MUI_DESCRIPTION_TEXT ${UninstSettings} "Uninstalls all settings and snippets from the install directory and user's application data directory."
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END

;--------------------------------

; Event functions

Function .onInit
	
	; Check if the installer is already running
	System::Call 'kernel32::CreateMutexA(i 0, i 0, t "${DIST_NAME} ${VERSION}") i .r1 ?e'
	Pop $0
	StrCmp $0 0 +3
	MessageBox MB_OK|MB_ICONSTOP "The ${DIST_NAME} ${VERSION} installer is already running."
	Abort
	
	Call GetDotNETVersion
	Pop $0
	${If} $0 == "not_found"
	MessageBox MB_OK|MB_ICONSTOP "You need to install Microsoft.NET 3.5 runtime before installing ${DIST_NAME}."
	${Else}
	${VersionCompare} $0 "3.5" $1
	${If} $1 == 2
	MessageBox MB_OK|MB_ICONSTOP "You need to install Microsoft.NET 3.5 runtime before installing ${DIST_NAME}. You have $0."
	${EndIf}
	${EndIf}
	
	Call GetFDInstDir
	Pop $0
	Call GetNeedsReset
	Pop $2
	${If} $2 == "do_reset"
	${If} $0 != "not_found"
	MessageBox MB_OK|MB_ICONEXCLAMATION "You have a version of ${DIST_NAME} installed that may make ${DIST_NAME} unstable or you may miss new features if updated. You should backup you custom setting files and do a full uninstall before installing this one. After install customize the new setting files."
	${EndIf}
	${EndIf}
	
	Call GetFlashVersion
	Pop $0
	${If} $0 == "not_found"
	MessageBox MB_OK|MB_ICONEXCLAMATION "You should install Flash Player (ActiveX for IE) before installing ${DIST_NAME}."
	${Else}
	${VersionCompare} $0 "9.0" $1
	${If} $1 == 2
	MessageBox MB_OK|MB_ICONEXCLAMATION "You should install Flash Player (ActiveX for IE) before installing ${DIST_NAME}. You have $0."
	${EndIf}
	${EndIf}
	
	Call GetJavaVersion
	Pop $0
	${If} $0 == "not_found"
	MessageBox MB_OK|MB_ICONEXCLAMATION "You should install 32-bit Java Runtime (1.6 or later) before installing ${DIST_NAME}."
	${Else}
	${VersionCompare} $0 "1.6" $1
	${If} $1 == 2
	MessageBox MB_OK|MB_ICONEXCLAMATION "You should install 32-bit Java Runtime (1.6 or later) before installing ${DIST_NAME}. You have $0."
	${EndIf}
	${EndIf}
	
	; Default to English
	StrCpy $1 ${NoChangesLocale}
	call .onSelChange
	
FunctionEnd

Function .onSelChange

	${If} ${SectionIsSelected} ${LanguageGroup}
	!insertmacro UnSelectSection ${LanguageGroup}
	!insertmacro SelectSection $1
	${Else}
	!insertmacro StartRadioButtons $1
	!insertmacro RadioButton ${NoChangesLocale}
	!insertmacro RadioButton ${EnglishLocale}
	!insertmacro RadioButton ${ChineseLocale}
	!insertmacro RadioButton ${JapaneseLocale}
	!insertmacro RadioButton ${GermanLocale}
	!insertmacro RadioButton ${BasqueLocale}
	!insertmacro RadioButton ${KoreanLocale}
	!insertmacro EndRadioButtons
	${EndIf}
	
FunctionEnd

;--------------------------------
