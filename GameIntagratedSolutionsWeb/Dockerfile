FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GameIntagratedSolutionsWeb/GameIntagratedSolutionsWeb.csproj", "GameIntagratedSolutionsWeb/"]
RUN dotnet restore "GameIntagratedSolutionsWeb/GameIntagratedSolutionsWeb.csproj"
COPY . .
WORKDIR "/src/GameIntagratedSolutionsWeb"
RUN dotnet build "GameIntagratedSolutionsWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameIntagratedSolutionsWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameIntagratedSolutionsWeb.dll"]
