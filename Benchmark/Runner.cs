using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RGeoIP;
using StackExchange.Redis;

namespace Benchmark
{
    public class Runner

    {
        ConnectionMultiplexer _conn;
        GeoIP _geoIP;

        public async Task<int> ImportAsync()
        {
            _conn = ConnectionMultiplexer.Connect("localhost:6379, allowadmin=true");
            _geoIP = new GeoIP(() => _conn.GetDatabase());
            using (var reader = new StreamReader("GeoIPCountryWhois.csv"))
            {
                var count = await _geoIP.ImportGeoLiteLegacyAsync(reader);
                return count;
            }
        }

        public async Task<TimeSpan> RunAsync()
        {
            var sw = Stopwatch.StartNew();
            var _ = await _geoIP.LookupAsync("1.1.2.245");
            sw.Stop();

            return sw.Elapsed;
        }

        public Task FlushDatabase()
        {
            return _conn.GetServer("localhost:6379").FlushDatabaseAsync();
        }
    }
}
