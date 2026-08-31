FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Security/IdentityServer/IdentityServer.csproj", "Security/IdentityServer/"]
RUN dotnet restore "Security/IdentityServer/IdentityServer.csproj"
COPY . .
WORKDIR "/src/Security/IdentityServer"
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]

