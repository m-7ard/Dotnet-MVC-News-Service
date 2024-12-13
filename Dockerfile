# Use the .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app


# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Use the .NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory and copy the published app
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port on which the app runs
EXPOSE 8080

# Set the entry point for the application
ENTRYPOINT ["dotnet", "MVC_News.MVC.dll"]