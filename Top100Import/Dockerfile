FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine3.9 AS build
LABEL cache kpearson/Top100Import-build

#Restore
WORKDIR /Top100Common
ADD Top100Common/Top100Common.csproj /Top100Common/

WORKDIR /Top100Import
ADD Top100Import/Top100Import.csproj /Top100Import/
RUN ["dotnet", "restore"]

#Build
ADD Top100Import /Top100Import
ADD Top100Common /Top100Common
RUN ["dotnet", "build", "-c", "Release"]
RUN ["dotnet", "publish", "-c", "Release", "-o", "/Top100Import"]

#Run
FROM mcr.microsoft.com/dotnet/core/runtime:3.0-alpine3.9
WORKDIR /Top100Import
COPY --from=build /Top100Import .

ENTRYPOINT ["dotnet", "Top100Import.dll", "top100.csv"]
