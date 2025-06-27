# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MeraStore.Services.Cross.Cutting.Api/MeraStore.Services.Cross.Cutting.Api.csproj", "src/MeraStore.Services.Cross.Cutting.Api/"]
COPY ["aspire/ms-cross-cutting-services.ServiceDefaults/ms-cross-cutting-services.ServiceDefaults.csproj", "aspire/ms-cross-cutting-services.ServiceDefaults/"]
COPY ["src/MeraStore.Services.Cross.Cutting.Infrastructure/MeraStore.Services.Cross.Cutting.Infrastructure.csproj", "src/MeraStore.Services.Cross.Cutting.Infrastructure/"]
COPY ["src/MeraStore.Services.Cross.Cutting.Application/MeraStore.Services.Cross.Cutting.Application.csproj", "src/MeraStore.Services.Cross.Cutting.Application/"]
COPY ["src/MeraStore.Services.Cross.Cutting.Domain/MeraStore.Services.Cross.Cutting.Domain.csproj", "src/MeraStore.Services.Cross.Cutting.Domain/"]
RUN dotnet restore "./src/MeraStore.Services.Cross.Cutting.Api/MeraStore.Services.Cross.Cutting.Api.csproj"
COPY . .
WORKDIR "/src/src/MeraStore.Services.Cross.Cutting.Api"
RUN dotnet build "./MeraStore.Services.Cross.Cutting.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

ARG NUGET_PASSWORD
RUN dotnet nuget add source "https://pkgs.dev.azure.com/merastore/MeraStore/_packaging/mera-store-feed/nuget/v3/index.json" --name "mera-store" --username "sanjyotagureddy" --password $NUGET_PASSWORD --store-password-in-clear-text

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeraStore.Services.Cross.Cutting.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeraStore.Services.Cross.Cutting.Api.dll"]