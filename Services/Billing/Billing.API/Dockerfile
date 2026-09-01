FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5005
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Services/Billing/Billing.API/Billing.API.csproj", "Services/Billing/Billing.API/"]
COPY ["Services/Billing/Billing.Application/Billing.Application.csproj", "Services/Billing/Billing.Application/"]
COPY ["Services/Billing/Billing.Domain/Billing.Domain.csproj", "Services/Billing/Billing.Domain/"]
COPY ["Services/Billing/Billing.Infrastructure/Billing.Infrastructure.csproj", "Services/Billing/Billing.Infrastructure/"]
COPY ["Common/EventBus.Messages/EventBus.Messages.csproj", "Common/EventBus.Messages/"]
RUN dotnet restore "Services/Billing/Billing.API/Billing.API.csproj"
COPY . .
WORKDIR "/src/Services/Billing/Billing.API"
RUN dotnet publish "Billing.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Billing.API.dll"]
