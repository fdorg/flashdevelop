AIR for Android instructions

1. Configuration:

	- edit 'bat\SetupSDK.bat' for paths to Flex SDK and Android SDK (default should be ok)
	
	- install your device's USB drivers:
	    http://developer.android.com/sdk/oem-usb.html
	- enable "USB debugging" on your Android device:
	    Parameters > Applications > Development > USB Debugging


2. Creating a self-signed certificate:

	- run 'bat\CreateCertificate.bat' to generate your self-signed certificate,

	(!) wait a minute before packaging.


3. Build from FlashDevelop as usual (F8)


4. Run/debug the application on the desktop as usual (F5 or Ctrl+Enter)


5. Install AIR runtime on your device:

	- run 'bat\InstallAirRuntime.bat'


6. Running/debugging the application on the device:

	6.a. Build/Debug directly on device
	- edit 'Run.bat' and change the run target 'goto desktop' by 'goto android-debug'
	- build & run as usual (Ctrl+Enter or F5) to package, install & run the application on your device
	
	6.b. Debug occasionally on device
	- Debug-build from FlashDevelop (F8)
	- run 'PackageApp.bat' to package and install a debug version of the application
	- start FlashDevelop debugger: Debug > Start Remote Session
	- start the application on device
	- the application should connect to FlashDevelop interactive debugger as usual


7. Packaging for release:

	- Release-build from FlashDevelop (F8)
	- run 'PackageApp.bat' and select Android/normal target
