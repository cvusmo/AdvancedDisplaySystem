@echo off
if [%2] == [] goto EOF

echo Parameters: %*

set SRC=%~p1
set NAME=%~2

set TARGET=C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64\Plugins\Local
mkdir %TARGET% >NUL 2>&1

echo.
echo Deploying CLIENT plugin binary:
echo.
:RETRY
ping -n 2 127.0.0.1 >NUL 2>&1
echo From %1 to "%TARGET%\"
copy /y %1 "%TARGET%\"

IF %ERRORLEVEL% NEQ 0 GOTO :RETRY
echo Done
echo.
exit 0

:EOF