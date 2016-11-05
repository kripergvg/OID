using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Cli.Utils;
using OID.Core;
using OID.Core.HashGenerator;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.User.In;
using OID.Web.Models;
using Constants = OID.Web.Core.Constants;

namespace OID.Web.Controllers
{
    public class UserController : OIDController
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IHashGenerator _hashGenerator;
        private readonly IUserManager _userManager;
        private readonly IUserProvider _userProvider;
        private readonly IRegionProvider _regionProvider;
        private readonly IMapper _mapper;

        public UserController(ISessionProvider sessionProvider, IHashGenerator hashGenerator, IUserManager userManager, IUserProvider userProvider,
            IRegionProvider regionProvider, IMapper mapper)
        {
            _sessionProvider = sessionProvider;
            _hashGenerator = hashGenerator;
            _userManager = userManager;
            _userProvider = userProvider;
            _regionProvider = regionProvider;
            _mapper = mapper;
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
            var model = new ManageUserViewModel();

            var userTask = _userProvider.GetUser();
            var regionsTask = _regionProvider.GetRegions();
            var userPhonesTask = _userProvider.GetUserPhones();

            var user = (await userTask).Model;

            string regionCode = String.IsNullOrEmpty(user.RegionCode) ? Constants.MOSCOW_KLADR : user.RegionCode;
            model.RegionCode = regionCode;
            model.RegionList = new SelectList((await regionsTask).Model, "Code", "Name", regionCode);

            var userPhones = (await userPhonesTask).Model;

            model.PhoneMobile = _mapper.Map<PhoneViewModel>(userPhones.Mobile);
            model.PhoneWork = _mapper.Map<PhoneViewModel>(userPhones.Work);
            model.PhoneHome = _mapper.Map<PhoneViewModel>(userPhones.Home);
            model.PhoneAdditional = _mapper.Map<PhoneViewModel>(userPhones.Additional);

            var deleveryType = AddressType.City;
            if (user.DeleveryLocationType.HasValue)
            {
                deleveryType = (AddressType) user.DeleveryLocationType;
            }

            model.DeliveryLocationType = deleveryType;

            var regionTask = _regionProvider.GetLocalities(regionCode);
            var cityTask = _regionProvider.GetLocationsByRegion(regionCode);

            var localitiesRequest = await regionTask;
            string localityCode = String.IsNullOrEmpty(user.LocalityCode) ? localitiesRequest.Model.First().Code : user.LocalityCode;

            var cityRequest = await cityTask;
            string cityCode = String.IsNullOrEmpty(user.CityCode) ? cityRequest.Model.First().Code : user.CityCode;

            model.City = new ManageUserViewModel.CityViewModel
            {
                CityCode = cityCode,
                CityList = new SelectList(cityRequest.Model, "Code", "Name", cityCode)
            };

            var locationRequest = await _regionProvider.GetLocationsByLocality(localityCode);
            string locationCode = String.IsNullOrEmpty(user.CityCode) ? locationRequest.Model.First().Code : user.CityCode;

            model.Locality = new ManageUserViewModel.LocalityViewModel
            {
                LocalityList = new SelectList(localitiesRequest.Model, "Code", "Name", localityCode),
                LocalityCode = user.LocalityCode,
                LocationList = new SelectList(locationRequest.Model, "Code", "Name", locationCode),
                LocationCode = user.CityCode
            };

            model.Address = user.Address;

            return View(model);
        }

        [HttpPost]
        [Authorize("HasSessionID")]
        public async Task<IActionResult> Manage(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userTask = _userProvider.GetUser();
                var userPhonesTask = _userProvider.GetUserPhones();

                var user = (await userTask).Model;
                var oldUserModel=new UserContactsModel(
                {
                    Address = user.Address,
                    LocalityCode = user.LocalityCode,
                }


                var userPhones = (await userPhonesTask).Model;

                _userProvider.UpsertUserContacts()
            }
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
