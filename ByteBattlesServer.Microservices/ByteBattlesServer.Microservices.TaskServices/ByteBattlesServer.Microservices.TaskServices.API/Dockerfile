FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
 
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.API/ByteBattlesServer.Microservices.TaskServices.API.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.API/"]
COPY ["ByteBattlesServer.SharedContracts/ByteBattlesServer.SharedContracts.csproj", "ByteBattlesServer.SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/ByteBattlesServer.Microservices.TaskServices.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Infrastructure/ByteBattlesServer.Microservices.TaskServices.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Infrastructure/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Application/ByteBattlesServer.Microservices.TaskServices.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Application/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.API/ByteBattlesServer.Microservices.TaskServices.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.API"
RUN dotnet build "./ByteBattlesServer.Microservices.TaskServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattlesServer.Microservices.TaskServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattlesServer.Microservices.TaskServices.API.dll"]
