using System;
using System.IO;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace RGeoIP.Tests
{
    public class GeoIPTests : IDisposable
    {
        private ConnectionMultiplexer _conn;
        private GeoIP _geoIP;

        public GeoIPTests()
        {
            _conn = ConnectionMultiplexer.Connect("localhost:6379, allowadmin=true");
            _geoIP = new GeoIP(() => _conn.GetDatabase());
            using (var reader = new StreamReader("GeoIPCountryWhois-small.csv"))
            {
                _geoIP.ImportGeoLiteLegacyAsync(reader).Wait();
            }
        }

        [Fact]
        public async Task Should_have_imported_data()
        {
            var count = await _geoIP.CountAsync();

            Assert.Equal(60, count);
        }

        [Fact]
        public async Task Should_be_able_to_lookup_IP_within_range()
        {
            var auAddresses = new[] { "1.0.0.24", "1.0.4.245", "1.4.0.222" };
            foreach (var ip in auAddresses)
            {
                var au = await _geoIP.LookupAsync(ip);
                Assert.NotNull(au);
                Assert.Equal("AU", au.Code);
                Assert.Equal("Australia", au.Name);
            }

            var cnAddresses = new[] { "1.0.1.245", "1.1.2.245", "1.1.62.255" };
            foreach (var ip in cnAddresses)
            {
                var cn = await _geoIP.LookupAsync(ip);
                Assert.NotNull(cn);
                Assert.Equal("CN", cn.Code);
                Assert.Equal("China", cn.Name);
            }
        }

        [Fact]
        public async Task Should_throw_exception_for_invalid_ip_address()
        {
            Exception thrown = null;
            try
            {
                await _geoIP.LookupAsync("255.255.255.256");
            }
            catch (FormatException ex)
            {
                thrown = ex;
            }

            Assert.NotNull(thrown);
        }

        public void Dispose()
        {
            _conn.GetServer("localhost:6379").FlushDatabase();
        }
    }
}
