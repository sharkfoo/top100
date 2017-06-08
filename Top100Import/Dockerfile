FROM microsoft/dotnet:1.1.2-sdk

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
RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

#Run
WORKDIR /Top100Import/out/
ENTRYPOINT ["dotnet", "Top100Import.dll"]
