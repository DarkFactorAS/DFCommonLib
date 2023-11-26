# Use .Net Core 7 image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy files
COPY ./ ./

# Restore and build web
RUN dotnet restore DFCommonLib.TestApp/DFCommonLib.TestApp.csproj
RUN dotnet publish DFCommonLib.TestApp/DFCommonLib.TestApp.csproj -c Release -o out

# Create self signed sertificate
RUN dotnet dev-certs https -ep /app/out/certificate.pfx -p smurfepoliz
RUN dotnet dev-certs https --trust

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DFCommonLib.dll"]

EXPOSE 5100:80
