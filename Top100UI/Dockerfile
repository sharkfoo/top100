FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
LABEL cache=sharkfoo/top100ui-build

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
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ARG port
WORKDIR /Top100UI
COPY --from=build /Top100UI .

EXPOSE ${port}
ENV ASPNETCORE_URLS=http://*:${port}
ENTRYPOINT ["dotnet", "Top100UI.dll", "--server.urls", "http://+:${port}"]
