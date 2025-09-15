FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TheHireFactory.ECommerce.Domain/ ./TheHireFactory.ECommerce.Domain
COPY TheHireFactory.ECommerce.Infrastructure/ ./TheHireFactory.ECommerce.Infrastructure
COPY TheHireFactory.ECommerce.Api/ ./TheHireFactory.ECommerce.Api

RUN dotnet restore ./TheHireFactory.ECommerce.Api/TheHireFactory.ECommerce.Api.csproj
RUN dotnet publish ./TheHireFactory.ECommerce.Api/TheHireFactory.ECommerce.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TheHireFactory.ECommerce.Api.dll"]