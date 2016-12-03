using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OID.Core;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;
using OID.Web.Authenticate;
using OID.Web.Models;

namespace OID.Web.Controllers
{
    [Authorize("HasSessionID")]
    public class ObjectController : OIDController
    {
        private readonly IDealObjectProvider _dealObjectProvider;
        private readonly IMapper _mapper;

        public ObjectController(IDealObjectProvider dealObjectProvider, IMapper mapper)
        {
            _dealObjectProvider = dealObjectProvider;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var objects = await _dealObjectProvider.GetUserObjects();
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

        public async Task<IActionResult> Create()
        {
            var model = new ObjectModifyViewModel();

            var categories = await _dealObjectProvider.GetCategories();

            model.CategoryList = new SelectList(categories.Model, nameof(ObjectCategory.Id), nameof(ObjectCategory.Name));
            model.ObjectStatusType = ObjectStatusType.WantSell;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ObjectModifyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checks = model.ObjectChecks.Select(c => new DealCheck(((int) c.CheckType).ToString(), c.Description));
                var createObjectModel = new CreateDealObject(model.CategoryCode, ((int) model.ObjectStatusType).ToString(), model.Name, model.Description, checks);
                var createObjectResult = await _dealObjectProvider.CreateObject(createObjectModel);

                if (createObjectResult.ResultMessage.MessageType != MessageType.Error)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ShowError(createObjectResult.ResultMessage.Message);

                }
            }

            return View(model);
        }

        public async Task<IActionResult> Update(string objectID, string checkListId)
        {
            var categoriesTask = _dealObjectProvider.GetCategories();
            var checksTask = _dealObjectProvider.GetChecks(checkListId);
            var userObjectTask = _dealObjectProvider.GetUserObject(objectID);

            await Task.WhenAll(categoriesTask, checksTask, userObjectTask);

            var userObject = userObjectTask.Result.Model;

            var modifyModel = new ObjectModifyViewModel
            {
                CategoryList = new SelectList(categoriesTask.Result.Model, nameof(ObjectCategory.Id), nameof(ObjectCategory.Name)),
                ObjectChecks = _mapper.Map<IList<ObjectModifyViewModel.ObjectCheck>>(checksTask.Result.Model),
                Name = userObject.Name,
                ObjectStatusType = (ObjectStatusType) Int32.Parse(userObject.StatusId),
                Description = userObject.Description,
                CategoryCode = userObject.CategoryId
            };

            var model = new UpdateObjectViewModel
            {
                ModifyViewModel = modifyModel,
                ObjectId = objectID,
                CheckListId = checkListId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string objectID, string checkListId, ObjectModifyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checks = model
                    .ObjectChecks
                    .Select(c => new DealCheck(((int) c.CheckType).ToString(), c.Description, c.CheckId))
                    .ToList();

                var checksToAdd = checks
                    .Where(c => String.IsNullOrEmpty(c.CheckId))
                    .ToList();
                var addChecksTask = _dealObjectProvider.CreateChecks(checksToAdd, checkListId, objectID);

                var checksIds = checks
                    .Where(c => !String.IsNullOrEmpty(c.CheckId)).Select(c => c.CheckId);

                var oldChecks = await _dealObjectProvider.GetChecks(checkListId);
                var oldChecksIds = oldChecks.Model.Select(c => c.CheckId);

                var checksToDelete = oldChecksIds.Where(c => !checksIds.Contains(c)).ToList();
                var deleteChecksTask = _dealObjectProvider.DeleteChecks(checksToDelete, checkListId, objectID);

                var updateObjectModel = new UpdateDealObject(objectID, model.CategoryCode, ((int) model.ObjectStatusType).ToString(), model.Name, model.Description);
                var updateObjectTask = _dealObjectProvider.UpdateObject(updateObjectModel);

                await Task.WhenAll(addChecksTask, deleteChecksTask, updateObjectTask);

                if (addChecksTask.Result.ResultMessage.MessageType != MessageType.Error
                    && deleteChecksTask.Result.ResultMessage.MessageType != MessageType.Error
                    && updateObjectTask.Result.ResultMessage.MessageType != MessageType.Error)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ShowError(updateObjectTask.Result.ResultMessage.Message);

                }
            }

            var categories = await _dealObjectProvider.GetCategories();
            model.CategoryList = new SelectList(categories.Model, nameof(ObjectCategory.Id), nameof(ObjectCategory.Name));

            return View(new UpdateObjectViewModel
            {
                CheckListId = checkListId,
                ModifyViewModel = model,
                ObjectId = objectID
            });
        }

        public async Task<IActionResult> Copy(string objectID, string checkListId)
        {
            var categoriesTask = _dealObjectProvider.GetCategories();
            var checksTask = _dealObjectProvider.GetChecks(checkListId);
            var userObjectTask = _dealObjectProvider.GetUserObject(objectID);

            await Task.WhenAll(categoriesTask, checksTask, userObjectTask);

            var userObject = userObjectTask.Result.Model;

            var model = new ObjectModifyViewModel
            {
                CategoryList = new SelectList(categoriesTask.Result.Model, nameof(ObjectCategory.Id), nameof(ObjectCategory.Name)),
                ObjectChecks = _mapper.Map<IList<ObjectModifyViewModel.ObjectCheck>>(checksTask.Result.Model),
                Name = userObject.Name,
                ObjectStatusType = (ObjectStatusType) Int32.Parse(userObject.StatusId),
                Description = userObject.Description,
                CategoryCode = userObject.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Copy(ObjectModifyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checks = model.ObjectChecks.Select(c => new DealCheck(((int) c.CheckType).ToString(), c.Description));
                var createObjectModel = new CreateDealObject(model.CategoryCode, ((int) model.ObjectStatusType).ToString(), model.Name, model.Description, checks);
                var createObjectResult = await _dealObjectProvider.CreateObject(createObjectModel);

                if (createObjectResult.ResultMessage.MessageType != MessageType.Error)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ShowError(createObjectResult.ResultMessage.Message);

                }
            }

            var categories = await _dealObjectProvider.GetCategories();
            model.CategoryList = new SelectList(categories.Model, nameof(ObjectCategory.Id), nameof(ObjectCategory.Name));

            return View(model);
        }

        public async Task<IActionResult> Delete()
        {
            return View();
        }
    }
}
