# 1. Etapa de compilación (Bypass Anti-Bloqueo de MCR)
FROM debian:bookworm-slim AS build
WORKDIR /src

# Agregamos 'libicu-dev' aquí para que el compilador de .NET pueda leer los textos
RUN apt-get update && apt-get install -y wget ca-certificates bash libicu-dev \
    && wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet

# Agregamos .NET al sistema
ENV PATH="${PATH}:/usr/share/dotnet"

COPY ["EscuelaManagement.csproj", "./"]
RUN dotnet restore "EscuelaManagement.csproj"
COPY . .
RUN dotnet publish "EscuelaManagement.csproj" -c Release -o /app/publish

# 2. Etapa de ejecución
FROM debian:bookworm-slim AS final
WORKDIR /app

# Instalamos dependencias del sistema, librerías de .NET y tu PARCHE GRÁFICO (libgdiplus y fuentes)
RUN apt-get update && apt-get install -y --no-install-recommends \
    wget \
    ca-certificates \
    bash \
    libicu-dev \
    libgdiplus \
    libc6-dev \
    fontconfig \
    libfontconfig1 \
    fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

# Descargamos el entorno de ejecución de ASP.NET 9.0 para que tu sistema corra
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --channel 9.0 --runtime aspnetcore --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh

ENV PATH="${PATH}:/usr/share/dotnet"

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EscuelaManagement.dll"]