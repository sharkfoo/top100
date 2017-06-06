FROM microsoft/dotnet:1.1-sdk

ARG port

#Restore
WORKDIR /Top100
ADD Top100.csproj /Top100/
RUN ["dotnet", "restore"]

#Build
ADD . /Top100
RUN ["dotnet", "build", "-c", "Release"]
RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

#Run
WORKDIR /Top100/out/
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "Top100.dll"]