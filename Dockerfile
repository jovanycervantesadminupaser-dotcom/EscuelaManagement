# 1. Etapa de compilación (Usando etiqueta específica para evitar el bloqueo)
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
WORKDIR /src
COPY ["EscuelaManagement.csproj", "./"]
RUN dotnet restore "EscuelaManagement.csproj"
COPY . .
RUN dotnet publish "EscuelaManagement.csproj" -c Release -o /app/publish

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS final
WORKDIR /app

# ---- ¡EL PARCHE GRÁFICO COMPLETO PARA PDF EN LINUX! ----
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    libc6-dev \
    fontconfig \
    libfontconfig1 \
    fonts-liberation \
    && rm -rf /var/lib/apt/lists/*
# --------------------------------------------------------

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EscuelaManagement.dll"]