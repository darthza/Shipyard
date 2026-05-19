FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Shipyard.sln ./
COPY src/Shipyard.Web/Shipyard.Web.csproj src/Shipyard.Web/
RUN dotnet restore Shipyard.sln

COPY . .
RUN dotnet publish src/Shipyard.Web/Shipyard.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Shipyard.Web.dll"]
