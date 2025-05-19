#Ues aspnet version 9 as our base image
FROM  mcr.microsoft.com/dotnet/sdk:9.0 AS build

#Goes to the app directory
WORKDIR /app

#Copy dependency
COPY *.csproj ./

#Install dependency
RUN dotnet restore

#Copy files
COPY . ./
RUN dotnet publish -c Release -o out

#Set port
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 9260
ENV ASPNETCORE_ENVIRONMENT=Development
ENV PORT=9260

CMD ["dotnet", "api.dll"]