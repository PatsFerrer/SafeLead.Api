# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SafeLead.Api.csproj", "./"]
RUN dotnet restore "SafeLead.Api.csproj"

COPY . .
RUN dotnet publish "SafeLead.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Runtime enxuto e seguro
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Boas práticas de segurança: roda o container sem permissão de root
USER $APP_UID

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SafeLead.Api.dll"]