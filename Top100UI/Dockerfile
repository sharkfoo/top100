FROM microsoft/dotnet:2.2-sdk-alpine

ARG port

#Restore
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
RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

#Run
WORKDIR /Top100UI/out/
EXPOSE ${port}/tcp
ENV ASPNETCORE_URLS http://*:${port}
ENTRYPOINT ["dotnet", "Top100UI.dll"]
