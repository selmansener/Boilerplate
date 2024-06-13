#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InvoiceFetcher.API/InvoiceFetcher.API.csproj", "InvoiceFetcher.API/"]
RUN dotnet restore "InvoiceFetcher.API/InvoiceFetcher.API.csproj"
COPY . .
WORKDIR "/src/InvoiceFetcher.API"
RUN dotnet build "InvoiceFetcher.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InvoiceFetcher.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvoiceFetcher.API.dll"]