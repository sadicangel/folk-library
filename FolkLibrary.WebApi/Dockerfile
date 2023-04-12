#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FolkLibrary.WebApi/FolkLibrary.WebApi.csproj", "FolkLibrary.WebApi/"]
COPY ["FolkLibrary.Application/FolkLibrary.Application.csproj", "FolkLibrary.Application/"]
COPY ["FolkLibrary.Domain/FolkLibrary.Domain.csproj", "FolkLibrary.Domain/"]
COPY ["FolkLibrary.Infrastructure/FolkLibrary.Infrastructure.csproj", "FolkLibrary.Infrastructure/"]
RUN dotnet restore "FolkLibrary.WebApi/FolkLibrary.WebApi.csproj"
COPY . .
WORKDIR "/src/FolkLibrary.WebApi"
RUN dotnet build "FolkLibrary.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FolkLibrary.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FolkLibrary.WebApi.dll"]