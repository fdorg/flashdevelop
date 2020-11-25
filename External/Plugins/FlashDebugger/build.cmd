@echo off

set FLEX_HOME="..\..\..\FlashDevelop\Bin\Debug\Tools\flexlibs"
set JAVA_HOME="C:\Program Files\Java\jdk1.8.0_144"
set PATH=C:\Program Files\Java\jdk1.8.0_144\bin

mkdir generated

proxygen.exe fdb.proxygen.xml
IF %ERRORLEVEL% NEQ 0 goto err


mkdir generated\bin

javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\events\*.java
IF %ERRORLEVEL% NEQ 0 goto err
::javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\threadsafe\*.java
::IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\expression\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\java_\io\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\java_\net\*.java
IF %ERRORLEVEL% NEQ 0 goto err
::javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flex\tools\debugger\cli\*.java
::IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.9.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\concrete\*.java
IF %ERRORLEVEL% NEQ 0 goto err

jar cvf fbd.j4n.jar -C generated\bin "."
IF %ERRORLEVEL% NEQ 0 goto err

C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /debug+ /nologo /warn:0 /t:library /out:fdb.j4n.dll /recurse:generated\clr\*.cs  /reference:"jni4net.n-0.8.9.0.dll"
::csc /debug+ /nologo /warn:0 /t:library /out:fdb.j4n.dll /recurse:generated\clr\*.cs  /reference:"jni4net.n-0.8.4.0.dll"
IF %ERRORLEVEL% NEQ 0 goto err

pause
exit 0

:err
pause
exit 1