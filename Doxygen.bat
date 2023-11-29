@echo off

if "%~1"=="clean" (
	REM wipe the html dir so we don't accrue unused files in the repository
	echo Clean ...
	if exist "./html" (RD /S /Q "./html")
)

REM run doxygen with the doxyfile in the same folder
"C:\Program Files\doxygen\bin\doxygen.exe"
