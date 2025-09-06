# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use ASP.NET Core runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Cloud Run specific environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV FIREBASE_PROJECT_ID=todolistfirebaseapp-d4e9a

# Expose port 8080 (Cloud Run standard)
EXPOSE 8080

# Health check for Cloud Run
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

# Note: Set FIREBASE_SERVICE_ACCOUNT_KEY environment variable when deploying to Cloud Run
# Example: gcloud run deploy --set-env-vars="FIREBASE_SERVICE_ACCOUNT_KEY=YOUR_JSON_KEY"

# Start the app
ENTRYPOINT ["dotnet", "TodoListFirebaseApp.dll"]
