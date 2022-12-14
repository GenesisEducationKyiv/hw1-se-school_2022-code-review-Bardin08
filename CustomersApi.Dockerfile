FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

COPY ./src/Genesis.Case/ ./
RUN dotnet restore Genesis.Case.sln

COPY ./src/Genesis.Case/customers/Api ./
RUN dotnet publish ./customers/Api/Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Api.dll"]
