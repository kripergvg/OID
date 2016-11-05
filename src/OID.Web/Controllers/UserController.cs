using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OID.Core.HashGenerator;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.Web.Authenticate;
using OID.Web.Models;

namespace OID.Web.Controllers
{
    public class UserController : OIDController
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IHashGenerator _hashGenerator;
        private readonly IUserManager _userManager;
        private readonly IUserProvider _userProvider;
        private readonly IRegionProvider _regionProvider;

        public UserController(ISessionProvider sessionProvider, IHashGenerator hashGenerator, IUserManager userManager, IUserProvider userProvider, IRegionProvider regionProvider)
        {
            _sessionProvider = sessionProvider;
            _hashGenerator = hashGenerator;
            _userManager = userManager;
            _userProvider = userProvider;
            _regionProvider = regionProvider;
        }

        // GET: /<controller>/
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModelViewModel model, string returnUrl = null)
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

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var passwordHash = _hashGenerator.Generate(model.Password, true);
                var registerResult = await _userProvider.CreateUser(model.Email, model.Name, _hashGenerator.Generate(passwordHash, true));

                if (registerResult.ResultMessage.MessageType != MessageType.Error)
                {
                    var userModel = new UserModel(model.Email, passwordHash, registerResult.Model.SessionId);
                    _userManager.SetUser(userModel);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, registerResult.ResultMessage.Message);
                }
            }

            return View(model);
        }

        [Authorize("HasSessionID")]
        public IActionResult LogOff()
        {
            var user = _userManager.GetUser();
            _sessionProvider.CloseSession(user.SessionId);
            _userManager.RemoveUser();

            return RedirectToLocal(null);
        }

        [Authorize("HasSessionID")]
        public async Task<IActionResult> Manage()
        {
            //var user = _userManager.GetUser();

            var model = new ManageUserViewModel();
            //var fullUserRequest = await _userProvider.GetUser(user);
            //_userManager.UpdateSessionId(fullUserRequest);

            //var fullUser = fullUserRequest.Model;

            //var deleveryType = AddressType.City;
            //if (fullUser.DeleveryLocationType.HasValue)
            //{
            //    deleveryType = (AddressType) fullUser.DeleveryLocationType;
            //}

            //model.DeliveryLocationType = deleveryType

            //if (!String.IsNullOrEmpty(fullUser.RegionCode))
            //{
            //    model.RegionCode = fullUser.RegionCode;

            //    var localitiesRequest = await _regionProvider.GetLocalities(fullUser.RegionCode);
            //    model.
            //}

            //var regions = await _regionProvider.GetRegions();
            

            return View(model);
        }

        [HttpPost]
        [Authorize("HasSessionID")]
        public IActionResult Manage(ManageUserViewModel model)
        {
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToIndex();
            }
        }
    }
}
