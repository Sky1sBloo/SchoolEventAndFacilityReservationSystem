FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY SFERS/*.csproj ./
RUN dotnet restore SFERS.csproj

COPY SFERS/ ./
RUN dotnet publish SFERS.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 5116
ENTRYPOINT ["dotnet", "SFERS.dll"]
