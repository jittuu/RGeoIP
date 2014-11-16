using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RGeoIP {
    public class Importer {
        IDatabase _redis;

        public Importer(IDatabase redis) {
            _redis = redis;
        }

        public async Task<int> ImportAsync(IEnumerable<IPCountry> ipCountries) {
            var count = 0;
            foreach (var r in ipCountries) {
                var fields = new[]{
                        new HashEntry("code", r.Code),
                        new HashEntry("country", r.Name)
                    };
                await _redis.HashSetAsync("geoip:" + r.Code, fields);

                var name = r.Code + ":" + count.ToString();
                await _redis.SortedSetAddAsync("geoip:index", name, r.MaxIPNumber);

                count++;
            }

            return count;
        }
    }
}