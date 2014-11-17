using CsvHelper.Configuration;

namespace RGeoIP
{
    public class GeoLiteLegacyMap : CsvClassMap<IPCountry>
    {
        public GeoLiteLegacyMap()
        {
            Map(m => m.MinIPAddress).Index(0);
            Map(m => m.MaxIPAddress).Index(1);
            Map(m => m.MinIPNumber).Index(2);
            Map(m => m.MaxIPNumber).Index(3);
            Map(m => m.Code).Index(4);
            Map(m => m.Name).Index(5);
        }
    }
}
