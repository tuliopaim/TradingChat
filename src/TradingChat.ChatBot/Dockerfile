#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TradingChat.ChatBot/TradingChat.ChatBot.csproj", "src/TradingChat.ChatBot/"]
COPY ["src/TradingChat.ChatBot.Commands/TradingChat.ChatBot.Commands.csproj", "src/TradingChat.ChatBot.Commands/"]
COPY ["src/TradingChat.Core/TradingChat.Core.csproj", "src/TradingChat.Core/"]
COPY ["src/TradingChat.ChatBot.External.Stooq/TradingChat.ChatBot.External.Stooq.csproj", "src/TradingChat.ChatBot.External.Stooq/"]
COPY ["src/TradingChat.Core.Rabbit/TradingChat.Core.Rabbit.csproj", "src/TradingChat.Core.Rabbit/"]
RUN dotnet restore "src/TradingChat.ChatBot/TradingChat.ChatBot.csproj"
COPY . .
WORKDIR "/src/src/TradingChat.ChatBot"
RUN dotnet build "TradingChat.ChatBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradingChat.ChatBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradingChat.ChatBot.dll"]