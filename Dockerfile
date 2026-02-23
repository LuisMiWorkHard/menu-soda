FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MenuSoda.csproj","."]
RUN dotnet restore "./MenuSoda.csproj"
COPY . .
RUN dotnet publish "./MenuSoda.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "MenuSoda.dll"]