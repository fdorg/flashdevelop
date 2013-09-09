AIR for iOS instructions

1. Configuration:

	- edit 'bat\SetupSDK.bat' for path to Flex SDK (defaults should be ok)


3. Build from FlashDevelop as usual (F8)


4. Run/debug the application on the desktop as usual (F5 or Ctrl+Enter)


5. Configure for iOS packaging in 'bat\SetupApplication.bat':
	
	Take a deep breath, pay the Apple tax and read extra carefully this tutorial:
	- http://www.codeandvisual.com/2011/exporting-for-iphone-using-air-27-and-flashdevelop-part-three-generating-developer-certificates-provisioning-profiles-and-p12-files/
	
	Now this is how to create the p12 key entirely on Windows (steps 1. to 8.):
	- http://connorullmann.com/2011/04/air-2-6-and-ios/
	
	Then for each project you'll have to go to on Apple's iOS Provisioning Portal:
	- create a new App ID with: name of the project and ID indicated in 'application.xml',
	- create a new Provisioning Profile: select App ID & registered devices that will be allowed to install the app.
	
	Finally you can complete:
	- IOS_DEV_CERT_FILE: path to your iOS developer 'p12' key (keep safe outside of project)
	- IOS_DEV_CERT_PASS: developer certificate's password
	    if you don't set it, remove "-storepass %IOS_DEV_CERT_PASS%" from the IOS_SIGNING_OPTIONS,
		you'll be prompted to type it when packaging.
	- IOS_PROVISION: the project's Provisioning Profile file (copy in project)


6. Running/debugging the application on the device:

	6.a. Build/Debug on device
	- edit 'Run.bat' and change the run target 'goto desktop' by 'goto ios-debug'
	- build as usual (Ctrl+Enter or F5) to package
	- you'll still have to manually upload & run the app on the device
	- the application should connect to FlashDevelop interactive debugger as usual
	
	6.b. Debug occasionally on device
	- Debug-build from FlashDevelop (F8)
	- run 'PackageApp.bat' to package and install a debug version of the application
	- start FlashDevelop debugger: Debug > Start Remote Session
	- start the application on device
	- the application should connect to FlashDevelop interactive debugger as usual


7. Packaging for release:

	- edit in 'bat\SetupApplication.bat' to add your "distribution" certificate (IOS_DIST_CERT_FILE)
	  Note: you can package ad-hoc IPAs using your developer certificate.
	
	- Release-build from FlashDevelop (F8)
	- run 'PackageApp.bat' and select 
	    either iOS/"ad-hoc" for installation on test devices
	    or iOS/App Store for upload in the iOS App Store.

Tips:
- iFunBox: iTunes replacement; installs app faster even if app version doesn't change,
- TestFlightApp: ad-hoc distribution service http://testflightapp.com
- HockeyKit: self hosted ad-hoc distribution https://github.com/TheRealKerni/HockeyKit
- Manual ad-hoc distribution: http://samvermette.com/71
