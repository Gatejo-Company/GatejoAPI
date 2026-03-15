# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY GatejoAPI.sln ./
COPY global.json ./
COPY GatejoAPI/GatejoAPI.csproj GatejoAPI/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY Persistence/Persistence.csproj Persistence/
COPY Utils/Utils.csproj Utils/

# Restore dependencies
RUN dotnet restore GatejoAPI.sln

# Copy source code
COPY . .

# Publish in Release mode
RUN dotnet publish GatejoAPI/GatejoAPI.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "API.GatejoAPI.dll"]
