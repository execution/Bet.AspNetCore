# Bet.AspNetCore.LetsEncrypt

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Bet.AspNetCore/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/fo9rakj7s7uhs3ij?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-aspnetcore)
[![NuGet](https://img.shields.io/nuget/v/Bet.AspNetCore.LetsEncrypt.svg)](https://www.nuget.org/packages?q=Bet.AspNetCore.LetsEncrypt)
![Nuget](https://img.shields.io/nuget/dt/Bet.AspNetCore.LetsEncrypt)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/bet-aspnetcore/shield/Bet.AspNetCore.LetsEncrypt/latest)](https://f.feedz.io/kdcllc/bet-aspnetcore/packages/Bet.AspNetCore.LetsEncrypt/latest/download)

*Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/bet-aspnetcore/nuget/index.json).*

This library is designed only to be used with ASP.NET Core with Kestrel Applications, no proxies.

## Install

```csharp
    dotnet add package Bet.AspNetCore.LetsEncrypt
```

Account and Certificate information can be stored:

1. In Memory
2. Azure Storage

The case usage is with Azure Container Instances (ACIs) for a small sized websites please referrer to sample application code.

## For usage of this library please refer to `LetsEncryptWeb` Sample project

1. [`LetsEncryptWeb`](../LetsEncryptWeb/)

## Testing the Code

1. Install and run `ngrok http -bind-tls=false 5500`

2. Add the generated code to  `hosts file`

```txt
    127.0.0.1 TMP.ngrok.io
```

- Windows location     `C:\Windows\System32\Drivers\etc\hosts`
- OS/Linux `sudo vim /etc/hosts`

By default, certificates generated by Let's Encrypt's staging certificates will not appear as a trusted certificate.

Navigate to : `https://TMP.ngrok.io:5501/weatherforecast`

## Future work

It would be beneficial to have the ability to:

1. Save to a fileshare
2. Wild card SSL certificates
3. CLI utility to generate SSL certificates based on DNS entries [Sample code](https://github.com/jwendl/keyvault-demo.git)