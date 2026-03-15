# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything and restore
COPY . .
RUN dotnet restore GatejoAPI.sln

# Publish in Release mode
RUN dotnet publish GatejoAPI/GatejoAPI.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Release

EXPOSE 8080

ENTRYPOINT ["dotnet", "API.GatejoAPI.dll"]
