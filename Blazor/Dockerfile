FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /app
EXPOSE 80

COPY ["WorkplaceAdministrator.Blazor/WorkplaceAdministrator.Blazor.csproj", "WorkplaceAdministrator.Blazor/"]
RUN dotnet build "WorkplaceAdministrator.Blazor.csproj" -c Release -o /app/build
COPY . .
WORKDIR "/src/WorkplaceAdministrator.Blazor"
RUN dotnet build "WorkplaceAdministrator.Blazor.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "WorkplaceAdministrator.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish/ .
ENTRYPOINT ["dotnet", "WorkplaceAdministrator.Api.dll"]