FROM microsoft/dotnet:2.0-sdk

ARG port

#Restore
#Restore
WORKDIR /Top100Common
ADD Top100Common/Top100Common.csproj /Top100Common/

WORKDIR /Top100
ADD Top100/Top100.csproj /Top100/
RUN ["dotnet", "restore"]

#Build
ADD Top100 /Top100
ADD Top100Common /Top100Common
RUN ["dotnet", "build", "-c", "Release"]
RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

#Run
WORKDIR /Top100/out/
EXPOSE ${port}/tcp
ENV ASPNETCORE_URLS http://*:${port}
ENTRYPOINT ["dotnet", "Top100.dll"]
