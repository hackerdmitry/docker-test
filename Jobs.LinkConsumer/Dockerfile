FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app

COPY Jobs.Core ./Jobs.Core
COPY Jobs.LinkConsumer ./Jobs.LinkConsumer
COPY DockerTest.Data ./DockerTest.Data
WORKDIR ./Jobs.LinkConsumer
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

WORKDIR /app/out
ENTRYPOINT ["dotnet", "Jobs.LinkConsumer.dll"]