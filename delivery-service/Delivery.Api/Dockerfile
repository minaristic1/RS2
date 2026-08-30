FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Delivery.Api/ Delivery.Api/
COPY Delivery.Data/ Delivery.Data/
WORKDIR /src/Delivery.Api
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Delivery.Api.dll"]
