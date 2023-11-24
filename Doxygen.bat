@echo off

REM wipe the html dir so we don't accrue unused files in the repository
if exist "Docs~/html" (RD /S /Q "Docs~/html")

REM run doxygen with the doxyfile in the same folder
"C:\Program Files\doxygen\bin\doxygen.exe"
