using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Region;

namespace OID.DataProvider.Interfaces
{
    public interface IRegionProvider
    {
        Task<DataProviderModel<List<Region>>> GetRegions();

        Task<DataProviderModel<List<Locality>>> GetLocalities(string regionCode);

        Task<DataProviderModel<List<City>>> GetCities(string regionCode);

        Task<DataProviderModel<List<Location>>> GetLocationsByRegion(string regionCode);

        Task<DataProviderModel<List<Location>>> GetLocationsByLocality(string localityCode);
    }
}
