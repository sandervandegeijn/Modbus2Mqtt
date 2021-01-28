FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Modbus2Mqtt/*.csproj /app/Modbus2Mqtt/
RUN dotnet restore

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./dotnetapp"]
