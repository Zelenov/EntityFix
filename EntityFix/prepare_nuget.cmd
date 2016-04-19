echo off
set cdd=%cd%
set cdd=%cdd:\=\\%

ECHO %cd%
FOR /F "tokens=2 delims==" %%I IN (
  'wmic datafile where "name='%cdd%\\EntityFix\\bin\\Release\\net40\\EntityFix.dll'" get version /format:list'
) DO SET "RESULT=%%I"
ECHO Publishing  EntityFix v.%RESULT%


ECHO Clearing 'lib'

rmdir /S /q nuget\lib
mkdir nuget\lib

ECHO Filling 'lib'
ROBOCOPY "EntityFix\bin\Release"  "nuget\lib" "EntityFix.dll" /E /njh /njs /ndl /nc /ns /nfl

ECHO Creating Package EntityFix.%RESULT%.nupkg
nuget pack "nuget\Package.nuspec" -o "nuget" -Version %RESULT%

ECHO DONE!