using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.Web.Authenticate;
using OID.Web.Models;

namespace OID.Web.Controllers
{
    [Authorize("HasSessionID")]
    public class ObjectListController : OIDController
    {
        private readonly IDealObjectProvider _dealObjectProvider;
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public ObjectListController(IDealObjectProvider dealObjectProvider, IUserManager userManager, IMapper mapper)
        {
            _dealObjectProvider = dealObjectProvider;
            _userManager = userManager;
            _mapper = mapper;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUser();
            var objects = await _dealObjectProvider.GetUserObjects(user);
            if (objects.ResultMessage.MessageType != MessageType.Error)
            {
                var model = _mapper.Map<List<ObjectListViewModel>>(objects.Model);
                return View(model);
            }
            else
            {
                ShowError(objects.ResultMessage.Message);
            }

            return RedirectToIndex();
        }
    }
}
