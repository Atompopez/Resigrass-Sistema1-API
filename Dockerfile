FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Resigrass-Sistema1-API/ResiGrass-API.csproj", "Resigrass-Sistema1-API/"]
RUN dotnet restore "Resigrass-Sistema1-API/ResiGrass-API.csproj"
COPY . .
WORKDIR "/src/Resigrass-Sistema1-API"
RUN dotnet build "ResiGrass-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResiGrass-API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResiGrass-API.dll"]