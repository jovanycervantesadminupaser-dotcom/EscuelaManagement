# 1. Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EscuelaManagement.csproj", "./"]
RUN dotnet restore "EscuelaManagement.csproj"
COPY . .
RUN dotnet publish "EscuelaManagement.csproj" -c Release -o /app/publish

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# ---- ¡EL PARCHE GRÁFICO PARA LOS PDF! ----
# Instalamos las fuentes y herramientas para que C# pueda dibujar en Linux
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*
# ------------------------------------------

COPY --from=build /app/publish .

# Puerto para Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EscuelaManagement.dll"]