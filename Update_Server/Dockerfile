#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Update_Server/Update_Server.csproj", "Update_Server/"]
RUN dotnet restore "Update_Server/Update_Server.csproj"
COPY . .
WORKDIR "/src/Update_Server"
RUN dotnet build "Update_Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Update_Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Update_Server.dll"]