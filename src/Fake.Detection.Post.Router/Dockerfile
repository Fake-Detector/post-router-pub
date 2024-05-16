FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5156;https://+:7156
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="123456"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/fakerouter.pfx
COPY ["fakerouter.pfx", "/https/fakerouter.pfx"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fake.Detection.Post.Router/Fake.Detection.Post.Router.csproj", "src/Fake.Detection.Post.Router/"]
COPY ["src/Fake.Detection.Post.Router.Messages/Fake.Detection.Post.Router.Messages.csproj", "src/Fake.Detection.Post.Router.Messages/"]
RUN dotnet nuget add source "https://nuget.pkg.github.com/Fake-Detector/index.json" -n github -u <USER> -p <PASSWORD> --store-password-in-clear-text
RUN dotnet restore "src/Fake.Detection.Post.Router/Fake.Detection.Post.Router.csproj"
COPY . .
WORKDIR "/src/src/Fake.Detection.Post.Router"
RUN dotnet build "Fake.Detection.Post.Router.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fake.Detection.Post.Router.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fake.Detection.Post.Router.dll"]
