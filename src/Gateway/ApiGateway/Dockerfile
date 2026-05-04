# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /repo

COPY src/Gateway/ApiGateway/ApiGateway.csproj src/Gateway/ApiGateway/
RUN dotnet restore src/Gateway/ApiGateway/ApiGateway.csproj

COPY src/Gateway/ApiGateway/ src/Gateway/ApiGateway/
RUN dotnet publish src/Gateway/ApiGateway/ApiGateway.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
