# School Facility and Equipment Reservation System

## Requirements
- dotnet

## Installation 
1. Request `.env` file for developers or define `SA_PASSWORD` in the `.env` file.
2. Build the program
```
dotnet build
```
3. Run the project
```
dotnet run --project ./SFERS/SFERS.csproj
```

### Setup with docker
1. Start and build container 
```
docker compose up --build
```
2. The respective database and webapp with be shown in the terminal
> See `docker-compose.yml` for ports
3. To shutdown
```
docker compose down
```

### Start migrations
1. Request the connection string from the developers
2. Add the user secrets **(you only need to do this once)**
```
dotnet user-secrets set "ConnectionStrings:SFERS_Db" <connection string>
```
3. Ensure that the container is running
```
docker compose up
```
4. The database is now port forwarded to localhost (see `docker-compose.yml` for ports). You can update the db with just
```
dotnet ef database update --project ./SFERS/SFERS.csproj
```
and their respective migration commands