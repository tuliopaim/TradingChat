#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TradingChat.WebApp/TradingChat.WebApp.csproj", "src/TradingChat.WebApp/"]
COPY ["src/TradingChat.Domain/TradingChat.Domain.csproj", "src/TradingChat.Domain/"]
COPY ["src/TradingChat.Core/TradingChat.Core.csproj", "src/TradingChat.Core/"]
COPY ["src/TradingChat.Infrastructure/TradingChat.Infrastructure.csproj", "src/TradingChat.Infrastructure/"]
COPY ["src/TradingChat.Application/TradingChat.Application.csproj", "src/TradingChat.Application/"]
COPY ["src/TradingChat.Core.Rabbit/TradingChat.Core.Rabbit.csproj", "src/TradingChat.Core.Rabbit/"]
RUN dotnet restore "src/TradingChat.WebApp/TradingChat.WebApp.csproj"
COPY . .
WORKDIR "/src/src/TradingChat.WebApp"
RUN dotnet build "TradingChat.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradingChat.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradingChat.WebApp.dll"]