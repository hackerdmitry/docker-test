FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
EXPOSE 80

COPY DockerTest.Web ./DockerTest.Web
COPY DockerTest.Data ./DockerTest.Data
WORKDIR ./DockerTest.Web
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["dotnet", "DockerTest.Web.dll"]