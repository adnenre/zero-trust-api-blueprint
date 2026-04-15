FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj"
RUN dotnet publish "src/ZeroTrustAPI.Api/ZeroTrustAPI.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ZeroTrustAPI.Api.dll"]