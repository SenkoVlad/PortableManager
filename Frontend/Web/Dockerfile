#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Frontend/Web/PortableManager.Web.Client.csproj", "Frontend/Web/"]
COPY ["PortableManager.Shared/PortableManager.Shared.csproj", "PortableManager.Shared/"]
RUN dotnet restore "Frontend/Web/PortableManager.Web.Client.csproj"
COPY . .
WORKDIR "/src/Frontend/Web"
RUN dotnet build "PortableManager.Web.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PortableManager.Web.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortableManager.Web.Client.dll"]