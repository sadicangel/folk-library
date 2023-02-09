FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FolkLibrary.Functions/FolkLibrary.Functions.csproj", "FolkLibrary.Functions/"]
RUN dotnet restore "FolkLibrary.Functions/FolkLibrary.Functions.csproj"
COPY . .
WORKDIR "/src/FolkLibrary.Functions"
RUN dotnet build "FolkLibrary.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FolkLibrary.Functions.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true