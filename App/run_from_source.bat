@echo off
setlocal
cd /d "%~dp0"
dotnet run --project FilamentDbApp\FilamentDbApp.csproj
pause
