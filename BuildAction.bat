@echo off

if "%1" == "Debug" (
    goto Build
) else if "%1" == "Release" (
    goto Build
)

exit /b

:Build
if NOT EXIST %~dp0\out (
    mkdir %~dp0\out
)

if NOT EXIST %~dp0\out\%1 (
    mkdir %~dp0\out\%1
)

del /S /Q %~dp0\out\%1

xcopy /E /Y /Q %~dp0\SettingsUI\bin\x64\%1\net9.0-windows10.0.19041.0\win-x64\ %~dp0\out\%1\
xcopy /E /Y /Q %~dp0\Tray\bin\%1\net9.0-windows\ %~dp0\out\%1\
xcopy /E /Y /Q %~dp0\WindowShot.Shared\bin\%1\net9.0\ %~dp0\out\%1\
xcopy /E /Y /Q %~dp0\WindowShotService\bin\%1\net9.0-windows\ %~dp0\out\%1\

exit /b