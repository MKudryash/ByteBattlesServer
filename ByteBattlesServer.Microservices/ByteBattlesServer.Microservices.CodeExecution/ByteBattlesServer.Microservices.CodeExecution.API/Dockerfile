FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS compile
USER root
# Устанавливаем Python, C, Java, C#
RUN apk add --no-cache gcc musl-dev python3 py3-pip openjdk11-jdk
ENV PATH=/root/.dotnet/tools:$PATH
RUN dotnet tool update -g dotnet-execute && echo 'export "PATH=$PATH:/root/.dotnet/tools"' >> /etc/profile

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM  compile AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 8085

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.API/ByteBattlesServer.Microservices.CodeExecution.API.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.API/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Application/ByteBattlesServer.Microservices.CodeExecution.Application.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Application/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Domain/ByteBattlesServer.Microservices.CodeExecution.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Infrastructure/ByteBattlesServer.Microservices.CodeExecution.Infrastructure.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.Infrastructure/"]
COPY ["ByteBattlesServer.Domain/ByteBattlesServer.Domain.csproj", "ByteBattlesServer.Domain/"]
COPY ["ByteBattlesServer.SharedContracts/ByteBattlesServer.SharedContracts.csproj", "ByteBattlesServer.SharedContracts/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/ByteBattlesServer.Microservices.Middleware.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.Middleware/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/ByteBattlesServer.Microservices.AuthService.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.AuthService.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/ByteBattlesServer.Microservices.UserProfile.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.UserProfile.Domain/"]
COPY ["ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/ByteBattlesServer.Microservices.TaskServices.Domain.csproj", "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.TaskServices/ByteBattlesServer.Microservices.TaskServices.Domain/"]
RUN dotnet restore "ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.API/ByteBattlesServer.Microservices.CodeExecution.API.csproj"
COPY . .
WORKDIR "/src/ByteBattlesServer.Microservices/ByteBattlesServer.Microservices.CodeExecution/ByteBattlesServer.Microservices.CodeExecution.API"
RUN dotnet build "./ByteBattlesServer.Microservices.CodeExecution.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ByteBattlesServer.Microservices.CodeExecution.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ByteBattlesServer.Microservices.CodeExecution.API.dll"]
