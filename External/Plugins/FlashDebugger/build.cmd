@echo off

set FLEX_HOME="..\..\..\FlashDEvelop\bin\Debug\Tools\flexlibs"

mkdir generated

proxygen.exe fdb.proxygen.xml
IF %ERRORLEVEL% NEQ 0 goto err


mkdir generated\bin

javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\events\*.java
IF %ERRORLEVEL% NEQ 0 goto err
::javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\threadsafe\*.java
::IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\expression\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\java_\io\*.java
IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\java_\net\*.java
IF %ERRORLEVEL% NEQ 0 goto err
::javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flex\tools\debugger\cli\*.java
::IF %ERRORLEVEL% NEQ 0 goto err
javac -nowarn -d generated\bin -sourcepath generated\jvm -cp "jni4net.j-0.8.8.0.jar";"%FLEX_HOME%\lib\fdb.jar" generated\jvm\flash\tools\debugger\concrete\*.java
IF %ERRORLEVEL% NEQ 0 goto err

jar cvf fbd.j4n.jar -C generated\bin "."
IF %ERRORLEVEL% NEQ 0 goto err

c:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /debug+ /nologo /warn:0 /t:library /out:fdb.j4n.dll /recurse:generated\clr\*.cs  /reference:"jni4net.n-0.8.8.0.dll"
::csc /debug+ /nologo /warn:0 /t:library /out:fdb.j4n.dll /recurse:generated\clr\*.cs  /reference:"jni4net.n-0.8.4.0.dll"
IF %ERRORLEVEL% NEQ 0 goto err

exit 0

:err
exit 1