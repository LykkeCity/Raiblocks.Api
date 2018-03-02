# Raiblocks.Api
API Service for Raiblocks (NANO)

# Prerequisites

- [ASP.NET Core 2](https://docs.microsoft.com/en-us/aspnet/core/getting-started)

# Running
 
Getting the repository:
```
git clone -b dev https://github.com/LykkeCity/Raiblocks.Api.git
cd ./Raiblocks.Api
git submodule init
git submodule update
```

Running:

```
export SettingsUrl=appsettings.json
cd ./Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi
dotnet restore
dotnet run
```
Go to [http://localhost:5000/swagger/ui/#/](http://localhost:5000/swagger/ui/#/)

# Environment setup

Path to [config file](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi/appsettings.json) is specified in enviroment variabled "SettingUrl".

DataConnString field contains connection string to Azure Table Storage. NodeURL field specifies the address of [RPC RaiBlocks](https://github.com/clemahieu/raiblocks/wiki/RPC-protocol).

# Development

Azure Table Storage is used to store data.

![Class diagram](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/ClassDiagram.gif)

# See also

 - [RaiBlocks Integration Sign Service](https://github.com/LykkeCity/Raiblocks.Sign/tree/dev)

 - [Test private RaiBlocks node with a custom generis block](https://github.com/artem-kruglov/raiblocks/tree/testnet)
