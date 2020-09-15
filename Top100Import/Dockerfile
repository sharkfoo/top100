FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
LABEL cache kpearson/top100import-build

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
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /Top100Import
COPY --from=build /Top100Import .

ENTRYPOINT ["dotnet", "Top100Import.dll", "top100.csv"]
