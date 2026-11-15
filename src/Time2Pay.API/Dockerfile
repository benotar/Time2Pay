FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
USER $APP_UID

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Time2Pay.API/Time2Pay.API.csproj", "Time2Pay.API/"]
COPY ["src/Time2Pay.BLL/Time2Pay.BLL.csproj", "Time2Pay.BLL/"]
COPY ["src/Time2Pay.DAL/Time2Pay.DAL.csproj", "Time2Pay.DAL/"]
COPY ["src/Time2Pay.DTO/Time2Pay.DTO.csproj", "Time2Pay.DTO/"]
COPY ["src/Time2Pay.Shared/Time2Pay.Shared.csproj", "Time2Pay.Shared/"]

RUN dotnet restore "./Time2Pay.API/Time2Pay.API.csproj"

COPY src/ .

WORKDIR "/src/Time2Pay.API"

RUN dotnet build "./Time2Pay.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Time2Pay.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Time2Pay.API.dll"]
