#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NetRabbitSample.Producer/NetRabbitSample.Producer.csproj", "NetRabbitSample.Producer/"]
RUN dotnet restore "NetRabbitSample.Producer/NetRabbitSample.Producer.csproj"
COPY . .
WORKDIR "/src/NetRabbitSample.Producer"
RUN dotnet build "NetRabbitSample.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetRabbitSample.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetRabbitSample.Producer.dll"]
