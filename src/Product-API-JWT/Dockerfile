FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5072

ENV ASPNETCORE_URLS=http://+:5072

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Product-API-JWT/Product-API-JWT.csproj", "src/Product-API-JWT/"]
RUN dotnet restore "src/Product-API-JWT/Product-API-JWT.csproj"
COPY . .
WORKDIR "/src/src/Product-API-JWT"
RUN dotnet build "Product-API-JWT.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Product-API-JWT.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product-API-JWT.dll"]
