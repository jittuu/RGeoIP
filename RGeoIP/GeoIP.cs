using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using StackExchange.Redis;

namespace RGeoIP {
    public class GeoIP {
        Func<IDatabase> _redis;

        public GeoIP(Func<IDatabase> redis) {
            _redis = redis;
        }

        public async Task<int> ImportGeoLiteLegacyAsync(TextReader reader)
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap(new GeoLiteLegacyMap());
            config.HasHeaderRecord = false;
            using (var rdr = new CsvReader(reader, config))
            {
                var result = await ImportAsync(rdr.GetRecords<IPCountry>());
                return result;
            }
        }

        public async Task<int> ImportAsync(IEnumerable<IPCountry> ipCountries) {
            var count = 0;
            foreach (var r in ipCountries) {
                var fields = new[]{
                        new HashEntry("code", r.Code),
                        new HashEntry("country", r.Name),
                        new HashEntry("min", r.MinIPAddress),
                        new HashEntry("max", r.MaxIPAddress)
                    };
                await _redis().HashSetAsync("geoip:" + r.Code, fields);

                var name = r.Code + ":" + count.ToString();
                await _redis().SortedSetAddAsync("geoip:index", name, r.MaxIPNumber);

                count++;
            }

            return count;
        }

        public Task<long> CountAsync()
        {
            return _redis().SortedSetLengthAsync("geoip:index");
        }

        public Task<IPCountry> LookupAsync(string ipAddress)
        {
            return LookupAsync(IPAddress.Parse(ipAddress));
        }

        public async Task<IPCountry> LookupAsync(IPAddress ip)
        {
            var num = ip.ToNumber();
            var values = await _redis().SortedSetRangeByScoreAsync("geoip:index", start: num, exclude: Exclude.Stop, take: 1);
            if (values.Length > 0)
            {
                string key = values[0];
                var hashes = await _redis().HashGetAllAsync("geoip:" + key.Substring(0, 2));

                var country = new IPCountry()
                {
                    Code = hashes.First(h => h.Name == "code").Value,
                    Name = hashes.First(h => h.Name == "country").Value,
                    MinIPAddress = hashes.First(h => h.Name == "min").Value,
                    MaxIPAddress = hashes.First(h => h.Name == "max").Value
                };

                country.MinIPNumber = IPAddress.Parse(country.MinIPAddress).ToNumber();
                country.MaxIPNumber = IPAddress.Parse(country.MaxIPAddress).ToNumber();

                return country;
            }


            return null;
        }
    }
}