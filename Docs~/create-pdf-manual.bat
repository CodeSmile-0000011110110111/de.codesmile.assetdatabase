@echo off
cd latex
call make.bat

cd..
cd..
echo.
xcopy "Docs~/latex/refman.pdf" MANUAL.pdf /Y /F
