#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 1. Build image(https://hub.docker.com/_/microsoft-dotnet-core-sdk/)
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

# Cache nuget restore
COPY ["src/UR.Server/UR.Server.csproj", "src/UR.Server/"]
COPY ["src/UR.Client/UR.Client.csproj", "src/UR.Client/"]
COPY ["src/UR/UR.csproj", "src/UR/"]
RUN dotnet restore "src/UR.Server/UR.Server.csproj"

# Copy sources and compile
COPY . .
WORKDIR "/src/src/UR.Server"
RUN dotnet build "UR.Server.csproj" -c Release -o /app/build

# 2. Release image
FROM build AS publish
RUN dotnet publish "UR.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# Copy content from Build image
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UR.Server.dll"]
