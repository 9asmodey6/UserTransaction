FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["UserTransaction.ServiceA/UserTransaction.ServiceA.csproj", "UserTransaction.ServiceA/"]
COPY ["UserTransaction.Shared/UserTransaction.Shared.csproj", "UserTransaction.Shared/"]
RUN dotnet restore "./UserTransaction.ServiceA/UserTransaction.ServiceA.csproj"

COPY . .
WORKDIR "/src/UserTransaction.ServiceA"
RUN dotnet build "./UserTransaction.ServiceA.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserTransaction.ServiceA.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserTransaction.ServiceA.dll"]