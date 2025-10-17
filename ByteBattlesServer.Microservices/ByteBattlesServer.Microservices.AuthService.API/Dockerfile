FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.API/ByteBattlesServer.Microservices.AuthService.API.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.API/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthServices.Application/ByteBattlesServer.Microservices.AuthServices.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthServices.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Application/ByteBattlesServer.Microservices.UserProfile.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Infrastructure/ByteBattlesServer.Microservices.AuthService.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Infrastructure/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.API/ByteBattlesServer.Microservices.AuthService.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.API"
RUN dotnet build "./ByteBattlesServer.Microservices.AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattlesServer.Microservices.AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattlesServer.Microservices.AuthService.API.dll"]
