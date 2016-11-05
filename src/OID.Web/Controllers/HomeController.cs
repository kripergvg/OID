using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OID.Web.Controllers
{
    public class HomeController : OIDController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
