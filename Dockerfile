FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

RUN adduser amorphie-shielduser --disabled-password --gecos "" && chown -R amorphie-shielduser:amorphie-shielduser /app
USER amorphie-shielduser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY ["amorphie.shield/amorphie.shield.csproj", "amorphie.shield/"]
RUN dotnet restore "amorphie.shield/amorphie.shield.csproj"
COPY . .
WORKDIR "/src/amorphie.shield"

ARG configuration=Release

RUN dotnet build "amorphie.shield.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "amorphie.shield.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "amorphie.shield.dll"]
