# Clear
rm -Rf bin/debug

# Flush Nuget repo
#dotnet nuget locals all -c

# Build and pack common lib
dotnet restore DFCommonLib.csproj
dotnet build DFCommonLib.csproj
dotnet pack DFCommonLib.csproj -o bin/debug

# Push packet
dotnet nuget push bin/debug/DarkFactor.Common.Lib.*.nupkg --api-key 1337 --source DarkFactor --skip-duplicate

read -p "Press any key to resume ..."
