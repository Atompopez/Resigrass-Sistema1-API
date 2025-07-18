# Usar .NET 8.0 SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 5024

# Copiar el archivo del proyecto y restaurar las dependencias
COPY ./ResiGrass-API.csproj ./ 
RUN dotnet restore

# Copiar el resto de la aplicación y publicarla en modo Release
COPY . ./ 
RUN dotnet publish -c Release -o out

# Usar .NET 8.0 runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Eliminar las dependencias de SkiaSharp y GDI+
# Ya no es necesario instalar libskia, libgdiplus u otras bibliotecas nativas.
# No se requiere instalación de dependencias gráficas adicionales.

# Copiar el archivo plantilla_certificado.docx al contenedor
COPY ./Util/plantilla_certificado.docx /app/Util/plantilla_certificado.docx

# Copiar la salida publicada y establecer el punto de entrada
COPY --from=build-env /app/out ./ 
ENTRYPOINT ["dotnet", "ResiGrass-API.dll"]
