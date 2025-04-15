# Etap build z SDK .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiowanie całego katalogu projektu do kontenera
COPY . . 

# Przywracanie zależności
RUN dotnet restore "WebApplication2.csproj"

# Budowanie aplikacji
RUN dotnet build "WebApplication2.csproj" -c Release -o /app/build

# Etap publikacji
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/build .

# Określenie portu, na którym aplikacja będzie nasłuchiwać
EXPOSE 80

# Uruchamianie aplikacji
ENTRYPOINT ["dotnet", "WebApplication2.dll"]
