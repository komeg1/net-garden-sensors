# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy the remaining source files
COPY . . 
RUN dotnet publish -c Release -o /publish

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Install curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the compiled output from the build stage
COPY --from=build /publish . 

# Set the entrypoint to run your app
ENTRYPOINT ["dotnet", "Api.dll"]
