using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sa3adaty.Areas.Admin.Models;
using Sa3adaty.Core;
using Sa3adaty.Core.Services;
using Sa3adaty.Core.ViewModels.Admin;
using Sa3adaty.Core.ViewModels.Admin.User;
using Sa3adaty.DAL.Infrastructure;
using WebMatrix.WebData;

namespace Sa3adaty.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        public ServicesManager servicesManager;
        public DataAccessManager dataManager;

         #region Constructor
        public UserController()
            {
                dataManager = new DataAccessManager();
                servicesManager = new ServicesManager(dataManager);
            }
        #endregion

        //
        //GET: /Admin/User/New
        public ActionResult New()
        {
            ViewBag.SelectedPage = Navigator.Items.NEWUSER;

            return View();
        }

        //
        //Post: /Admin/User/New
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(UserViewModel user, IEnumerable<HttpPostedFileBase> Images)
        {
            ViewBag.SelectedPage = Navigator.Items.NEWUSER;

            if (ModelState.IsValid)
            {
                if (servicesManager.AccountService.IsEmailExist(user.Email) > 0)
                {
                    TempData["ErrorMessage"] = "This email already exist, choose another email address";
                }
                else if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
                {
                    TempData["ErrorMessage"] = "User Failed To Add, Invalid Image File";
                }
                else
                {
                    WebSecurity.CreateUserAndAccount(user.Email, user.Password, new { Name = user.Name,Firstname= user.FirstName, LastName = user.LastName , MiddleName=user.MiddleName,DateOfBirth = user.DateOfBirth,Gender = user.Gender ,RegistrationDate = DateTime.Now  });
                    int new_id = servicesManager.AccountService.IsEmailExist(user.Email);

                    if (new_id > 0)
                    {
                        servicesManager.AccountService.AddUserImages(new_id, Images);
                        TempData["SuccessMessage"] = "User Added Successfully";
                        return RedirectToAction("Edit", new { id = new_id });
                    }
                    else
                        TempData["ErrorMessage"] = "User Failed To Add";
                }
            }

            return View();
        }

        //
        //GET: /Admin/User/Edit/{category-id}
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedPage = Navigator.Items.USERS;
            UserViewModel user = servicesManager.AccountService.GetUserById(id);

            if (user == null)
                return RedirectToAction("List");

            return View(user);
        }

        //
        //POST: /Admin/User/Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(UserViewModel user)
        {
            ViewBag.SelectedPage = Navigator.Items.USERS;
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                int new_id = servicesManager.AccountService.UpdateUser(user);
                if (new_id > 0)
                    TempData["SuccessMessage"] = "User Updated Successfully";
                else
                    TempData["ErrorMessage"] = "User Failed To Update";
            }
            return View(user);
        }

        public ActionResult List()
        {
            ViewBag.SelectedPage = Navigator.Items.LISTUSERS;
            return View();
        }

        public JsonResult _UsersList(int draw, int start = 0, int length = 2)
        {
            string search = Request.Form["search[value]"];
            string order_by = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            string order_dir = Request.Form["order[0][dir]"];

            DataTableViewModel result = new DataTableViewModel();
            result.draw = draw;
            int total_count;
            result.data = servicesManager.AccountService.GetUsers(out total_count, start, length, search, order_by, order_dir);

            result.recordsTotal = total_count;
            result.recordsFiltered = total_count;
            return Json(result);
        }

        [HttpPost]
        public JsonResult _NewImageAjax(int UserId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Images)
        {
            if (Images != null && Images.Count() > 0 && !ImageService.IsValid(Images))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                string user_name = servicesManager.AccountService.GetUserNameById(UserId);
                servicesManager.AccountService.AddUserImages(UserId, Images,"",Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public JsonResult _UpdateImageAjax(int ImageId, string Caption, string Description, IEnumerable<HttpPostedFileBase> Image)
        {
            if (Image != null && Image.Count() > 0 && !ImageService.IsValid(Image))
            {
                //error
                return Json(new { Success = false, Message = "Invalid image" });
            }
            else
            {
                string user_name = servicesManager.AccountService.GetUserNameByImageId(ImageId);
                servicesManager.AccountService.UpdateUserImage(ImageId, (Image != null ? Image.FirstOrDefault() : null),"", Caption, Description);
            }
            return Json(new { Success = true, Message = "Image Added Successfully" });
        }

        public ActionResult _UserImages(int UserId)
        {
            var view_model = servicesManager.AccountService.GetUserImages(UserId);
            return View("_ImagesList", view_model ?? new List<ImageViewModel>());
        }

        [HttpPost]
        public JsonResult _GetImage(int ImageId)
        {
            var view_model = servicesManager.AccountService.GetUserImage(ImageId);
            return Json(view_model);
        }

        [HttpPost]
        public JsonResult _DeleteImage(int image_id)
        {
            servicesManager.AccountService.DeleteUserimage(image_id);
            return Json(new { Success = true, image_id = image_id });
        }
    }
}
