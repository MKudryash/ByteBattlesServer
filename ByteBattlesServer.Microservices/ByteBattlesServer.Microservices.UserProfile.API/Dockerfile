FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8090

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.API/ByteBattlesServer.Microservices.UserProfile.API.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.API/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Application/ByteBattlesServer.Microservices.UserProfile.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Infrastructure/ByteBattlesServer.Microservices.UserProfile.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Infrastructure/"]
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.API/ByteBattlesServer.Microservices.UserProfile.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.API"
RUN dotnet build "./ByteBattlesServer.Microservices.UserProfile.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattlesServer.Microservices.UserProfile.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattlesServer.Microservices.UserProfile.API.dll"]
