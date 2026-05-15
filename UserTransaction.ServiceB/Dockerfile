FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["UserTransaction.ServiceB/UserTransaction.ServiceB.csproj", "UserTransaction.ServiceB/"]
COPY ["UserTransaction.Shared/UserTransaction.Shared.csproj", "UserTransaction.Shared/"]
RUN dotnet restore "./UserTransaction.ServiceB/UserTransaction.ServiceB.csproj"

COPY . .
WORKDIR "/src/UserTransaction.ServiceB"
RUN dotnet build "./UserTransaction.ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserTransaction.ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserTransaction.ServiceB.dll"]
