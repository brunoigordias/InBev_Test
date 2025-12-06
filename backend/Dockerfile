# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/InBev.API/InBev.API.csproj", "src/InBev.API/"]
COPY ["src/InBev.Application/InBev.Application.csproj", "src/InBev.Application/"]
COPY ["src/InBev.Domain/InBev.Domain.csproj", "src/InBev.Domain/"]
COPY ["src/InBev.Infrastructure/InBev.Infrastructure.csproj", "src/InBev.Infrastructure/"]

RUN dotnet restore "src/InBev.API/InBev.API.csproj"

# Copiar todo o código e compilar
COPY . .
WORKDIR "/src/src/InBev.API"
RUN dotnet build "InBev.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "InBev.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InBev.API.dll"]

