FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
LABEL cache=sharkfoo/top100sync-build

#Restore
WORKDIR /AppleScriptSlim
ADD AppleScriptSlim/AppleScriptSlim.csproj /AppleScriptSlim/

WORKDIR /Top100Common
ADD Top100Common/Top100Common.csproj /Top100Common/

WORKDIR /Top100Sync
ADD Top100Sync/Top100Sync.csproj /Top100Sync/
RUN ["dotnet", "restore"]

#Build
ADD AppleScriptSlim /AppleScriptSlim
ADD Top100Sync /Top100Sync
ADD Top100Common /Top100Common
RUN ["dotnet", "build", "-c", "Release"]
RUN ["dotnet", "publish", "-c", "Release", "-o", "/Top100Sync"]

#Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /Top100Sync
COPY --from=build /Top100Sync .

#ENTRYPOINT ["dotnet", "Top100Sync.dll", "2020.csv"]
ENTRYPOINT ["dotnet", "Top100Sync.dll", "top100.csv"]
