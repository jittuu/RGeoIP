RGeoIP
======

Store IP Ranges in Redis as sorted sets for fast lookup

Installation
------------
TODO: make nuget package

Usage
-----

Import [GeoLite](http://dev.maxmind.com/geoip/legacy/geolite/) legacy csv file

```csharp
var conn = ConnectionMultiplexer.Connect("localhost:6379, allowadmin=true");
var geoIP = new GeoIP(() => conn.GetDatabase());
using (var reader = new StreamReader("GeoIPCountryWhois.csv"))
{
  var count = await geoIP.ImportGeoLiteLegacyAsync(reader);
}
```

IP lookup for the country which took a few milliseconds to lookup with ( >100,000 data set). _In my windows laptop, it took ~10 ms._

```csharp
var country = await geoIP.LookupAsync("1.1.2.245");
```

Dependencies
------------

- [CsvHelper](https://github.com/JoshClose/CsvHelper)
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)

License
-------
[MIT](https://github.com/jittuu/RGeoIP/blob/master/LICENSE)
