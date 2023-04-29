FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /App

COPY ./TastyCook.UsersAPI ./TastyCook.UsersAPI
COPY ./Contracts ./Contracts

# Restore as distinct layers
RUN dotnet restore "./TastyCook.UsersAPI/TastyCook.UsersAPI.csproj"

# Build and publish a release
RUN dotnet publish "./TastyCook.UsersAPI/TastyCook.UsersAPI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /App

COPY --from=builder /App/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "TastyCook.UsersAPI.dll"]
