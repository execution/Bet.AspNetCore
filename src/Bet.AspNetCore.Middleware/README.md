# Bet.AspNetCore.Middleware

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.AspNetCore/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/fo9rakj7s7uhs3ij?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-aspnetcore)
[![NuGet](https://img.shields.io/nuget/v/Bet.AspNetCore.Middleware.svg)](https://www.nuget.org/packages?q=Bet.AspNetCore.Middleware)
![Nuget](https://img.shields.io/nuget/dt/Bet.AspNetCore.Middleware)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/bet-aspnetcore/shield/Bet.AspNetCore.Middleware/latest)](https://f.feedz.io/kdcllc/bet-aspnetcore/packages/Bet.AspNetCore.Middleware/latest/download)

*Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/bet-aspnetcore/nuget/index.json).*

The collection of AspNetCore Middlewares for Production and Development purposes.

## Install

```csharp
    dotnet add package Bet.AspNetCore.Middleware
```

## Usage

### `AddDeveloperListRegisteredServices`

Development middleware to display all of the DI services that were registered with `AspNetCore` application.

1. Add in `ConfigureServices`

```csharp
    services.AddDeveloperListRegisteredServices(o =>
    {
        o.PathOutputOptions = PathOutputOptions.Json;
    });
```

2. Add in `Configure`

```csharp
     app.UseDeveloperListRegisteredServices();
```
