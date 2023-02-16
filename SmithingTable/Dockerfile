FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENV ASPNETCORE_SERVER_URLS="http://+:80"
ENV DOTNET_URLS=http://+:80
ENTRYPOINT ["dotnet", "SmithingTable.dll"]
