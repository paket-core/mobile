"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -property installationPath > tmpFile
set /p VSWHERE= < tmpFile
del tmpFile
call "%VSWHERE%\VC\Auxiliary\Build\vcvars64.bat"