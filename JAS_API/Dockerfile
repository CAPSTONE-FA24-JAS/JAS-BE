# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Development

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies for each project
COPY ["JAS_API/WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["JAS_API/Infrastructures/Infrastructures.csproj", "Infrastructures/"]
COPY ["JAS_API/Application/Application.csproj", "Application/"]
COPY ["JAS_API/Domain/Domain.csproj", "Domain/"]

# Restore dependencies with --disable-parallel to avoid conflicts in Docker
RUN dotnet restore "JAS_API/WebAPI/WebAPI.csproj" --disable-parallel

# Copy the rest of the project files
COPY ["JAS_API/", "./"]

# Set the working directory to the WebAPI project
WORKDIR "/src/WebAPI"

# Build the project in the specified configuration (Release by default)
RUN dotnet build "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "WebAPI.dll"] 
