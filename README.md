# SPFFlatten

Loads flattened SPF records from [AutoSPF](https://www.autospf.com/)'s API and inserts them into a PowerDNS zone via the PowerDNS API.

# Requirements
* [.NET 5.0 runtime](https://dotnet.microsoft.com/download/dotnet/5.0)
* PowerDNS server with a configured API key
* AutoSPF account

# Usage

1. Create a PowerDNS record for your source SPF record that you'd like to flatten (eg. `_spf-source.example.com`)
2. Sign up to AutoSPF using that domain
3. Generate an [AutoSPF API key](https://www.autospf.com/settings#/api)
4. Create a new PowerDNS **zone** specifically for the flattened SPF records (eg. `_spf.example.com`)
5. Modify `appsettings.json` to contain all the correct info:
```json
{
  "AutoSpfApiKey": "aaaaaaaaaaaaaaa",
  "AutoSpfDomain": "_spf-source.example.com",
  "DestinationZone": "_spf.example.com",
  "PowerDnsServer": "https://dns.example.com/",
  "PowerDnsApiKey": "bbbbbbbbb"
}
```
6. Run `dotnet run` in this directory
