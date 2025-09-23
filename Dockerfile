# ---- STEP 1: Build the project ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.sln .
COPY GestionStages/*.csproj GestionStages/
RUN dotnet restore

# Copy everything and build
COPY . .
WORKDIR /src/GestionStages
RUN dotnet publish -c Release -o /app

# ---- STEP 2: Run the project ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GestionStages.dll"]
