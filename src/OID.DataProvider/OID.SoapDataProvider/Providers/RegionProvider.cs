using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Region;
using OID.SoapDataProvider.Providers.Infrastructure;

namespace OID.SoapDataProvider.Providers
{
    public class RegionProvider : IRegionProvider
    {
        private readonly IAppQueryExecutorDecorator _queryExecutorDecorator;

        public RegionProvider(IAppQueryExecutorDecorator queryExecutorDecorator)
        {
            _queryExecutorDecorator = queryExecutorDecorator;
        }

        public async Task<DataProviderModel<List<Region>>> GetRegions()
        {
            List<Query> listQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetRegions");
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getRegionsQuery = result.Queries.FirstOrDefault(q => q.Name == "GetRegions");

            var regions = new List<Region>();
            if (getRegionsQuery != null)
            {
                var dataTable = getRegionsQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    regions.Add(new Region(
                        objectRow["CODE"].ToString(),
                        objectRow["NAME"].ToString()
                    ));
                }

            }

            return new DataProviderModel<List<Region>>(result.ResultMessage, regions);
        }

        public async Task<DataProviderModel<List<Locality>>> GetLocalities(string regionCode)
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetRegionLocalities");
            query.Parameters.Add(new QueryParameter("in", "Region_Code", regionCode));
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getLocalitiesQuery = result.Queries.FirstOrDefault(q => q.Name == "GetRegionLocalities");

            var localities = new List<Locality>();
            if (getLocalitiesQuery != null)
            {
                var dataTable = getLocalitiesQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    localities.Add(new Locality(
                        objectRow["CODE"].ToString(),
                        objectRow["NAME"].ToString()
                    ));
                }
            }

            return new DataProviderModel<List<Locality>>(result.ResultMessage, localities);
        }

        public async Task<DataProviderModel<List<City>>> GetCities(string regionCode)
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetRegionCities");
            query.Parameters.Add(new QueryParameter("in", "Region_Code", regionCode));
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getCitiesQuery = result.Queries.FirstOrDefault(q => q.Name == "GetRegionCities");

            var cities = new List<City>();
            if (getCitiesQuery != null)
            {
                var dataTable = getCitiesQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    cities.Add(new City(
                        objectRow["CODE"].ToString(),
                        objectRow["NAME"].ToString()
                    ));
                }
            }

            return new DataProviderModel<List<City>>(result.ResultMessage, cities);
        }

        public async Task<DataProviderModel<List<Location>>> GetLocationsByRegion(string regionCode)
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetRegionCities");
            query.Parameters.Add(new QueryParameter("in", "Region_Code", regionCode));
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getLocationsQuery = result.Queries.FirstOrDefault(q => q.Name == "GetRegionCities");

            var locations = new List<Location>();
            if (getLocationsQuery != null)
            {
                var dataTable = getLocationsQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    locations.Add(new Location(
                        objectRow["CODE"].ToString(),
                        objectRow["NAME"].ToString()
                    ));
                }
            }

            return new DataProviderModel<List<Location>>(result.ResultMessage, locations);
        }

        public async Task<DataProviderModel<List<Location>>> GetLocationsByLocality(string localityCode)
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetRegionCities");
            query.Parameters.Add(new QueryParameter("in", "Locality_Code", localityCode));
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getLocationsQuery = result.Queries.FirstOrDefault(q => q.Name == "GetRegionCities");

            var locations = new List<Location>();
            if (getLocationsQuery != null)
            {
                var dataTable = getLocationsQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    locations.Add(new Location(
                        objectRow["CODE"].ToString(),
                        objectRow["NAME"].ToString()
                    ));
                }
            }

            return new DataProviderModel<List<Location>>(result.ResultMessage, locations);
        }
    }
}
