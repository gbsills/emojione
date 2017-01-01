@echo off
pushd %~dp0

:BuildRelease
"%ProgramFiles(x86)%\MSBuild\14.0\bin\MSBuild.exe" build.proj /v:m
if %ERRORLEVEL% neq 0 goto BuildFail
goto BuildSuccess

:BuildFail
echo.
echo *** BUILD FAILED WITH ERRORLEVEL %ERRORLEVEL% ***
goto End

:BuildSuccess
echo.
echo **** BUILD SUCCESSFUL ***
goto end

:End
popd
exit /b %ERRORLEVEL%
