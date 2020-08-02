# Flush Nuget repo
dotnet nuget locals all -c

# Build and pack common lib
dotnet restore DFCommonLib.csproj
dotnet build DFCommonLib.csproj
dotnet pack DFCommonLib.csproj -o ~/NuGet
