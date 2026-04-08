FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . . 

RUN dotnet publish "UniTrack.WebAPI/UniTrack.WebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Buradaki yolu da klasör yapına göre düzelttik
# COPY UniTrack.WebAPI/service-account-key.json .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UniTrack.WebAPI.dll"]