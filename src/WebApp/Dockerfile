FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY . ./
RUN ls
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore WebApp

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WebApp.dll"]
