FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UniTrack-Backend/ .

RUN dotnet publish "UniTrack.WebAPI/UniTrack.WebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY UniTrack-Backend/UniTrack.WebAPI/service-account-key.json .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UniTrack.WebAPI.dll"]