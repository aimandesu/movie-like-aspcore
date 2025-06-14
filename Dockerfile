# #Use aspnet version 9 as our base image
# FROM  mcr.microsoft.com/dotnet/sdk:9.0 AS build

# #Goes to the app directory
# WORKDIR /app

# #Copy dependency
# COPY *.csproj ./

# #Install dependency
# RUN dotnet restore

# #Copy files
# COPY . ./

# #Create wwwroot files
# RUN mkdir -p /app/wwwroot/

# RUN dotnet publish -c Release -o out

# #Set port
# FROM mcr.microsoft.com/dotnet/aspnet:9.0
# WORKDIR /app
# COPY --from=build /app/out .

# EXPOSE 8080
# ENV ASPNETCORE_ENVIRONMENT=Development

# CMD ["dotnet", "api.dll"]

# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy .csproj and restore first (for caching)
COPY api/*.csproj api/
COPY application/*.csproj application/
COPY infrastructure/*.csproj infrastructure/
COPY domain/*.csproj domain/
RUN dotnet restore api/api.csproj

# Copy the rest and build
COPY . .
WORKDIR /src/api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
