FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine3.9 AS build
LABEL cache kpearson/top100ui-build

#Restore
WORKDIR /Top100Common
ADD Top100Common/Top100Common.csproj /Top100Common/

WORKDIR /Top100UI
ADD Top100UI/Top100UI.csproj /Top100UI/
RUN ["dotnet", "restore"]

#Build
ADD Top100UI /Top100UI
ADD Top100Common /Top100Common
RUN ["dotnet", "build", "-c", "Release"]
RUN ["dotnet", "publish", "-c", "Release", "-o", "/Top100UI"]

#Run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine3.9
ARG port
WORKDIR /Top100UI
COPY --from=build /Top100UI .

EXPOSE ${port}/tcp
ENV ASPNETCORE_URLS http://*:${port}
ENTRYPOINT ["dotnet", "Top100UI.dll"]
