using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;
using OID.DataProvider.Models.User;
using OID.Web.Core;
using OID.Web.Models;
using OID.Web.Models.Deal;
using OID.Web.Models.Partials;
using OID.Web.Models.User;


namespace OID.Web.Controllers
{
    public class DealController : OIDController
    {
        private readonly IDealProvider _dealProvider;
        private readonly IMapper _mapper;
        private readonly IDealObjectProvider _dealObjectProvider;
        private readonly IUserProvider _userProvider;
        private readonly IRegionProvider _regionProvider;

        public DealController(IDealProvider dealProvider, IMapper mapper, IDealObjectProvider dealObjectProvider, IUserProvider userProvider, IRegionProvider regionProvider)
        {
            _dealProvider = dealProvider;
            _mapper = mapper;
            _dealObjectProvider = dealObjectProvider;
            _userProvider = userProvider;
            _regionProvider = regionProvider;
        }

        public async Task<IActionResult> Index()
        {
            var deals = await _dealProvider.GetDeals();
            if (deals.ResultMessage.MessageType != MessageType.Error)
            {
                var model = _mapper.Map<List<DealViewModel>>(deals.Model);
                return View(model);
            }
            else
            {
                ShowError(deals.ResultMessage.Message);
            }

            return RedirectToIndex();
        }

        public async Task<IActionResult> UpdateSell(int dealId)
        {
            var dealObjectsTask = _dealObjectProvider.GetDealObjects(dealId);
            var dealTask = _dealProvider.GetDeal(dealId);
            var dealDeliveryTask = _dealProvider.GetDealDelevery(dealId);
            var sellModifyModelTask = CreateSellModel();

            await Task.WhenAll(dealObjectsTask, dealObjectsTask, dealTask, sellModifyModelTask);

            var sellModifyModel = sellModifyModelTask.Result;
            var deal = dealTask.Result.Model;
            var dealDelevery = dealDeliveryTask.Result.Model;
            var dealObjects = dealObjectsTask.Result.Model;

            sellModifyModel.Comment = deal.Comment;
            sellModifyModel.Price = deal.Price;
            sellModifyModel.Size = dealDelevery.SizeDeclire;
            sellModifyModel.SelectedDealObjects = _mapper.Map<List<SellDealModifyViewModel.SelectedDealObject>>(dealObjects);
            sellModifyModel.Weight = dealDelevery.WeightDeclire;

            sellModifyModel.AccountNumber = deal.AccountId;
            if (deal.AccountId.HasValue)
            {
                sellModifyModel.AccountAction = AccountAction.Existed;

                foreach (var userAccount in sellModifyModel.UserAccounts)
                {
                    if (userAccount.Value == deal.AccountId.Value.ToString())
                    {
                        userAccount.Selected = true;
                    }
                }
            }

            sellModifyModel.DeleveryTypeId = dealDelevery.DeliveryCptyServiceId;
            foreach (var deleveryType in sellModifyModel.DeleveryTypes)
            {
                if (deleveryType.Value == dealDelevery.DeliveryCptyServiceId.ToString())
                {
                    deleveryType.Selected = true;
                }
            }

            var model = new UpdateSellDealViewModel
            {
                DealId = dealId,
                SellDealModel = sellModifyModel
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSell(int dealId, SellDealModifyViewModel modfyModel)
        {
            var currentDeliveryObjectsTask = _dealObjectProvider.GetDealObjects(dealId);
            var createUserAccountTask = Task.FromResult(new DataProviderModel<CreateUserAccountModel>(new ResultMessage(0, MessageType.Information, "")));
            if (modfyModel.AccountAction == AccountAction.New)
            {
                createUserAccountTask = _userProvider.CreateUserAccount(modfyModel.AccountNumber.Value, modfyModel.PaymentType);
            }

            await Task.WhenAll(currentDeliveryObjectsTask, createUserAccountTask);

            var currentDeliveryObjects = currentDeliveryObjectsTask.Result.Model;
            var createUserAccount = createUserAccountTask.Result.Model;

            var objectsToDelete = currentDeliveryObjects
                .Where(c => modfyModel.SelectedDealObjects.All(s => s.ObjectId != c.ObjectId))
                .Select(o => o.DealObjectId)
                .ToList();

            var objectsToAdd = modfyModel.SelectedDealObjects
                .Where(s => currentDeliveryObjects.All(c => c.ObjectId != s.ObjectId))
                .Select(o => o.ObjectId)
                .ToList();

            var accountId = modfyModel.AccountAction == AccountAction.Existed ? modfyModel.AccountNumber.Value : createUserAccount.AccountId;

            var updateModel = new DealUpdateModel(dealId, objectsToDelete, objectsToAdd, modfyModel.Price, modfyModel.Comment, accountId, modfyModel.DeleveryTypeId,
                modfyModel.Size, modfyModel.Weight, modfyModel.AddressModel.City.CityCode, modfyModel.AddressModel.Locality.LocalityCode,
                modfyModel.AddressModel.RegionCode, modfyModel.AddressModel.Address, (DeleveryLocationType) modfyModel.AddressModel.DeliveryLocationType);

            await _dealProvider.UpdateDeal(updateModel, DealType.Sell);
        
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateSell()
        {
            var model = await CreateSellModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSell(SellDealModifyViewModel model)
        {
            return View();
        }

        public async Task<IActionResult> ViewDeal(int dealId)
        {
            return View();
        }

        private async Task<SellDealModifyViewModel> CreateSellModel()
        {
            var freeObjectsTask = _dealObjectProvider.GetUserObjects(true, DealType.Sell);
            var userTask = _userProvider.GetUser();
            var regionsTask = _regionProvider.GetRegions();
            var deliveryTypesTask = _dealProvider.GetDeleveryTypes();
            var accountsTask = _userProvider.GetUserAccounts();

            await Task.WhenAll(freeObjectsTask, userTask, regionsTask, deliveryTypesTask, accountsTask);

            var user = userTask.Result.Model;

            string regionCode = String.IsNullOrEmpty(user.RegionCode) ? Constants.MOSCOW_KLADR : user.RegionCode;

            var locationTask = _regionProvider.GetLocalities(regionCode);
            var cityTask = _regionProvider.GetLocationsByRegion(regionCode);

            await Task.WhenAll(locationTask, cityTask);

            var freeObjects = freeObjectsTask.Result.Model;
            var locations = locationTask.Result.Model;
            var cities = cityTask.Result.Model;
            var regions = regionsTask.Result.Model;
            var deliveryTypes = deliveryTypesTask.Result.Model;
            var accounts = accountsTask.Result.Model;

            var deleveryType = AddressType.City;
            if (user.DeleveryLocationType.HasValue)
            {
                deleveryType = (AddressType) user.DeleveryLocationType;
            }

            string cityCode = String.IsNullOrEmpty(user.CityCode) ? cities.First().Code : user.CityCode;
            string localityCode = String.IsNullOrEmpty(user.LocalityCode) ? locations.First().Code : user.LocalityCode;
            string locationCode = String.IsNullOrEmpty(user.CityCode) ? locations.First().Code : user.CityCode;

            var accountsFormatted = accounts.Select(a => new SelectItem
            {
                Value = a.AccountId,
                Text = $"{a.PaymentService.GetHumanName()} {a.AccountNumber}"
            });

            return new SellDealModifyViewModel
            {
                AddressModel = new AddressViewModel(nameof(SellDealModifyViewModel.AddressModel))
                {
                    Address = user.Address,
                    DeliveryLocationType = deleveryType,
                    City = new CityViewModel
                    {
                        CityCode = cityCode,
                        CityList = new SelectList(cities.OrderBy(l => l.Name), "Code", "Name", cityCode)
                    },
                    Locality = new LocalityViewModel
                    {
                        LocalityList = new SelectList(locations.OrderBy(l => l.Name), "Code", "Name", localityCode),
                        LocalityCode = user.LocalityCode,
                        LocationList = new SelectList(locations.OrderBy(l => l.Name), "Code", "Name", locationCode),
                        LocationCode = user.CityCode
                    },
                    RegionCode = regionCode,
                    RegionList = new SelectList(regions.OrderBy(l => l.Name), "Code", "Name", regionCode),
                },
                DeleveryTypes = new SelectList(deliveryTypes, "DeliveryId", "Name"),
                FreeDealObjects = new SelectList(freeObjects, "ObjectId", "Name"),
                UserAccounts = new SelectList(accountsFormatted, nameof(SelectItem.Value), nameof(SelectItem.Text))
            };
        }
    }
}
