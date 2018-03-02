# Raiblocks.Api
API Service for Raiblocks (NANO)

# Prerequisites

- [ASP.NET Core 2](https://docs.microsoft.com/en-us/aspnet/core/getting-started)

# Running
 
Getting the repository:
```
git clone -b dev https://github.com/artem-kruglov/Raiblocks.Api.git
cd ./Raiblocks.ApiService
git submodule init
git submodule update
```

Running:
```
cd ./Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi
dotnet restore
dotnet run
```
Go to [http://localhost:5000/swagger/ui/#/](http://localhost:5000/swagger/ui/#/)

# Environment setup

The path to [config file](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi/appsettings.json) is specified in enviroment variabled "SettingUrl".

The field DataConnString contains connection string to Azure Table Storage. The field NodeURL specify the address of [RPC Raiblocks](https://github.com/clemahieu/raiblocks/wiki/RPC-protocol).

# Development

Azure Table Storage is uset to store data.

![Class diagram](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/ClassDiagram.gif)


