@echo off

if "%1" == "Debug" (
    goto Build
) else if "%1" == "Release" (
    goto Build
)

if "%1" == "Clean" (
    goto Clean
)

exit /b

:Clean
rmdir /S /Q %~dp0Build\bin
rmdir /S /Q %~dp0SettingsUI\bin
rmdir /S /Q %~dp0Tray\bin
rmdir /S /Q %~dp0Shared\bin
rmdir /S /Q %~dp0Service\bin
rmdir /S /Q %~dp0out\Debug
rmdir /S /Q %~dp0out\Release
exit /b

:Build
if NOT EXIST %~dp0out (
    mkdir %~dp0out
)

if NOT EXIST %~dp0\out\%1 (
    mkdir %~dp0out\%1
)

del /S /Q %~dp0out\%1

xcopy /E /Y /Q %~dp0SettingsUI\bin\x64\%1\net9.0-windows10.0.19041.0\win-x64\ %~dp0out\%1\
xcopy /E /Y /Q %~dp0Tray\bin\%1\net9.0-windows\ %~dp0out\%1\
xcopy /E /Y /Q %~dp0Shared\bin\%1\net9.0\ %~dp0out\%1\
xcopy /E /Y /Q %~dp0Service\bin\%1\net9.0-windows10.0.19041.0\ %~dp0out\%1\

exit /b