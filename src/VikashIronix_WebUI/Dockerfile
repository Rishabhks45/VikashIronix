FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/VikashIronix_WebUI/VikashIronix_WebUI.csproj"
RUN dotnet publish "src/VikashIronix_WebUI/VikashIronix_WebUI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5190
EXPOSE 5190

ENTRYPOINT ["dotnet", "VikashIronix_WebUI.dll"]
