FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY daemon-console/bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app
ENTRYPOINT ["dotnet", "daemon-console.dll"]