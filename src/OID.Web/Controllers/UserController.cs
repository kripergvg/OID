using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OID.Core.HashGenerator;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.Web.Authenticate;
using OID.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OID.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IHashGenerator _hashGenerator;
        private readonly IUserManager _userManager;

        public UserController(ISessionProvider sessionProvider, IHashGenerator hashGenerator, IUserManager userManager)
        {
            _sessionProvider = sessionProvider;
            _hashGenerator = hashGenerator;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var authenticateResult = await _sessionProvider.Authenticate(model.Email, _hashGenerator.Generate(model.Password, true));
                if (authenticateResult.ResultMessage.MessageType != MessageType.Error)
                {
                    var userModel = new UserModel(model.Email, _hashGenerator.Generate(model.Password, true), authenticateResult.Model.SessionId);
                    _userManager.SetUser(userModel);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, authenticateResult.ResultMessage.Message);
                }
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
