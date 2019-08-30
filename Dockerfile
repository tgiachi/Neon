FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 1883

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
RUN git clone https://github.com/tgiachi/Neon.git /src
RUN dotnet restore  
COPY . .
WORKDIR /src/Neon.WebApi
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 CMD [ "curl --fail http://localhost:5000/api/Health/Ping || exit 1" ]
ENTRYPOINT ["dotnet", "Neon.WebApi.dll"]