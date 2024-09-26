# Use .NET 8.0 SDK to build the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build-env
WORKDIR /app

# Copy project files and restore dependencies
COPY ResiGrass-API/*.csproj ./
RUN dotnet restore

# Copy the rest of the app and publish in Release mode
COPY ResiGrass-API/. ./
RUN dotnet publish -c Release -o out

# Use .NET 8.0 runtime to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output and set the entry point
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ResiGrass-API.dll"]