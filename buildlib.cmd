@echo off
REM Clear
rd /q /s .\bin\debug

REM Flush Nuget repo
dotnet nuget locals all -c

REM Build and pack common lib
dotnet restore DFCommonLib.csproj
dotnet build DFCommonLib.csproj
dotnet pack DFCommonLib.csproj -o bin/debug

REM Push packet
dotnet nuget push .\bin\debug\DarkFactor.Common.Lib.1.*.nupkg --api-key 1337 --source DarkFactor --skip-duplicate

