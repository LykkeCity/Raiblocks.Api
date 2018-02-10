# Raiblocks.Api
API Service for Raiblocks (NANO)

# Prerequisites

- [Visual Studio 2017](https://www.microsoft.com/net/core#windowsvs2017)
- [ASP.NET Core 2](https://docs.microsoft.com/en-us/aspnet/core/getting-started)

# Running
 
 Cloning:
```
git clone https://github.com/LykkeCity/Raiblocks.Api.git
cd ./Raiblocks.Api
git submodule init
git submodule update
```

Running:
```
cd ./Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi
dotnet restore
dotnet run
```
Open [http://localhost:5000/swagger/ui/#/](http://localhost:5000/swagger/ui/#/)

# Environment setup

Path to settins file is set in environment variable "SettingUrl". You should put there path to [config file](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/src/Lykke.Service.RaiblocksApi/appsettings.json).

DataConnString should contain connection string to Azure Table Storage. NodeURL should contain [RPC Raiblocks endpoint](https://github.com/clemahieu/raiblocks/wiki/RPC-protocol).

# Development

Azure Table Storage is uset to store data.

![Class diagram](https://github.com/LykkeCity/Raiblocks.Api/blob/dev/Lykke.Service.Raiblocks.Api/ClassDiagram.gif)


