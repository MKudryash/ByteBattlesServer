FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.API/ByteBattlesServer.Microservices.SolutionService.API.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.API/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Application/ByteBattlesServer.Microservices.SolutionService.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Domain/ByteBattlesServer.Microservices.SolutionService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Domain/"]
COPY ["ByteBattlesServer.SharedContracts/ByteBattlesServer.SharedContracts.csproj", "ByteBattlesServer.SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Infrastructure/ByteBattlesServer.Microservices.SolutionService.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.Infrastructure/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/ByteBattlesServer.Microservices.TaskServices.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.API/ByteBattlesServer.Microservices.SolutionService.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.SolutionService/ByteBattlesServer.Microservices.SolutionService.API"
RUN dotnet build "./ByteBattlesServer.Microservices.SolutionService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattlesServer.Microservices.SolutionService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattlesServer.Microservices.SolutionService.API.dll"]
