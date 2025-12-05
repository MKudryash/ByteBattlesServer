FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.API/ByteBattles.Microservices.CodeBattleServer.API.csproj", "ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.API/"]
COPY ["ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Application/ByteBattles.Microservices.CodeBattleServer.Application.csproj", "ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Domain/ByteBattles.Microservices.CodeBattleServer.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Domain/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/ByteBattlesServer.Microservices.TaskServices.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/"]
COPY ["ByteBattlesServer.SharedContracts/ByteBattlesServer.SharedContracts.csproj", "ByteBattlesServer.SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Infrastructure/ByteBattles.Microservices.CodeBattleServer.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.Infrastructure/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Domain/ByteBattlesServer.Microservices.SolutionService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.API/ByteBattles.Microservices.CodeBattleServer.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattles.Microservices.CodeBattleServer/ByteBattles.Microservices.CodeBattleServer.API"
RUN dotnet build "./ByteBattles.Microservices.CodeBattleServer.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattles.Microservices.CodeBattleServer.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattles.Microservices.CodeBattleServer.API.dll"]
