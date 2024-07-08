#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BallastLane.BookVault.Api/BallastLane.BookVault.Api.csproj", "BallastLane.BookVault.Api/"]
RUN dotnet restore "./BallastLane.BookVault.Api/BallastLane.BookVault.Api.csproj"
COPY . .
WORKDIR "/src/BallastLane.BookVault.Api"
RUN dotnet build "./BallastLane.BookVault.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BallastLane.BookVault.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BallastLane.BookVault.Api.dll"]