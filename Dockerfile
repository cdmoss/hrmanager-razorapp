FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WorkplaceAdministrator.Web/WorkplaceAdministrator.Web.csproj", "WorkplaceAdministrator.Web/"]
RUN dotnet restore "WorkplaceAdministrator.Web.csproj" -c Release -o /app/build
COPY . .
WORKDIR "/src/WorkplaceAdministrator.Web"
RUN dotnet build "WorkplaceAdministrator.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkplaceAdministrator.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish/ .
ENTRYPOINT ["dotnet", "WorkplaceAdministrator.Web.dll"]