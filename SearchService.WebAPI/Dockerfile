#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SearchService.WebAPI/SearchService.WebAPI.csproj", "SearchService.WebAPI/"]
COPY ["CommonInitializer/CommonInitializer.csproj", "CommonInitializer/"]
COPY ["Chen.ASPNETCore/Chen.ASPNETCore.csproj", "Chen.ASPNETCore/"]
COPY ["Chen.Commons/Chen.Commons.csproj", "Chen.Commons/"]
COPY ["Chen.EventBus/Chen.EventBus.csproj", "Chen.EventBus/"]
COPY ["Chen.Infrastructure/Chen.Infrastructure.csproj", "Chen.Infrastructure/"]
COPY ["Chen.DomainCommons/Chen.DomainCommons.csproj", "Chen.DomainCommons/"]
COPY ["Chen.JWT/Chen.JWT.csproj", "Chen.JWT/"]
COPY ["SearchService.Infrastructure/SearchService.Infrastructure.csproj", "SearchService.Infrastructure/"]
COPY ["SearchService.Domain/SearchService.Domain.csproj", "SearchService.Domain/"]
RUN dotnet restore "./SearchService.WebAPI/SearchService.WebAPI.csproj"
COPY . .
WORKDIR "/src/SearchService.WebAPI"
RUN dotnet build "./SearchService.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SearchService.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchService.WebAPI.dll"]