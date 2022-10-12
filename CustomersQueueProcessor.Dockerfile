FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

COPY ./src/Genesis.Case/ ./
RUN dotnet restore Genesis.Case.sln

COPY ./src/Genesis.Case/customers/Api ./
RUN dotnet publish ./Customers.Queue.Processor/Customers.Queue.Processor.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Customers.Queue.Processor.dll"]
