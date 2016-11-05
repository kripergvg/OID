using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OID.DataProvider.Interfaces;

namespace OID.Web.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionProvider _regionProvider;

        public RegionController(IRegionProvider regionProvider)
        {
            _regionProvider = regionProvider;
        }

        [HttpGet]
        public async Task<JsonResult> GetLocalities(string regionCode)
        {
            var localities = await _regionProvider.GetLocalities(regionCode);
            return Json(localities.Model.OrderBy(l => l.Name));
        }

        [HttpGet]
        public async Task<JsonResult> GetLocationsByRegion(string regionCode)
        {
            var locations = await _regionProvider.GetLocationsByRegion(regionCode);
            return Json(locations.Model.OrderBy(l=>l.Name));
        }

        [HttpGet]
        public async Task<JsonResult> GetLocationsByLocality(string localityCode)
        {
            var locations = await _regionProvider.GetLocationsByLocality(localityCode);
            return Json(locations.Model.OrderBy(l => l.Name));
        }
    }
}
