FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /App

COPY ./TastyCook.ProductsAPI ./TastyCook.ProductsAPI
COPY ./TastyCook.Contracts ./TastyCook.Contracts

# Restore as distinct layers
RUN dotnet restore "./TastyCook.ProductsAPI/TastyCook.ProductsAPI.csproj"

# Build and publish a release
RUN dotnet publish "./TastyCook.ProductsAPI/TastyCook.ProductsAPI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /App

COPY --from=builder /App/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "TastyCook.ProductsAPI.dll"]
