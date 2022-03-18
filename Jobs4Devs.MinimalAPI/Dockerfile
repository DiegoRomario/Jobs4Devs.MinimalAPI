#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Jobs4Devs.MinimalAPI/Jobs4Devs.MinimalAPI.csproj", "Jobs4Devs.MinimalAPI/"]
RUN dotnet restore "Jobs4Devs.MinimalAPI/Jobs4Devs.MinimalAPI.csproj"
COPY . .
WORKDIR "/src/Jobs4Devs.MinimalAPI"
RUN dotnet build "Jobs4Devs.MinimalAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jobs4Devs.MinimalAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jobs4Devs.MinimalAPI.dll"]