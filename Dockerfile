FROM mcr.microsoft.com/dotnet/aspnet:3.1

COPY daemon-console/bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app
ENTRYPOINT ["dotnet", "daemon-console.dll"]