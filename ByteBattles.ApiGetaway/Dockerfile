FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattles.ApiGetaway/ByteBattles.ApiGetaway.csproj", "ByteBattles.ApiGetaway/"]
COPY ["ByteBattlesServer.SharedContracts/ByteBattlesServer.SharedContracts.csproj", "ByteBattlesServer.SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/ByteBattlesServer.Microservices.TaskServices.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/"]
RUN dotnet restore "ByteBattles.ApiGetaway/ByteBattles.ApiGetaway.csproj"
COPY . .
WORKDIR "/src/ByteBattles.ApiGetaway"
RUN dotnet build "./ByteBattles.ApiGetaway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattles.ApiGetaway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattles.ApiGetaway.dll"]
