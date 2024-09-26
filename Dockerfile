# Usar .NET 8.0 SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar los archivos del proyecto y restaurar las dependencias
COPY ./Resigrass-api.csproj ./
RUN dotnet restore

# Copiar el resto de la aplicación y publicarla en modo Release
COPY . ./
RUN dotnet publish -c Release -o out

# Usar .NET 8.0 runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar la salida publicada y establecer el punto de entrada
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "Resigrass-api.dll"]