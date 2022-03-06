FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dol-hub/dol-hub.csproj", "dol-hub/"]
RUN dotnet restore "dol-hub/dol-hub.csproj"
COPY . .
WORKDIR "/src/dol-hub"
RUN dotnet build "dol-hub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dol-hub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dol-hub.dll"]
