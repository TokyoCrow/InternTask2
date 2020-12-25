using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.Website.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService db;

        public AdminController(IAdminService ias)
        {
            db = ias;
        }

        public ActionResult Approve(int id)
        {
            var user = db.GetUserById(id);
            if (user == null)
                return HttpNotFound();

            try
            {
                db.SendApproveEmail(user);
            }
            catch
            {
                return RedirectToAction("EmailSendingError");
            }

            return RedirectToAction("NewRegistrations");
        }

        public ActionResult Reject(int id)
        {
            var user = db.GetUserById(id);
            if (user == null)
                return HttpNotFound();

            try
            {
                db.SendRejectEmail(user);
            }
            catch
            {
                return RedirectToAction("EmailSendingError");
            }

            return RedirectToAction("NewRegistrations");
        }

        public ActionResult Registrations(int page = 1)
        {
            int pageSize = 15;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, UserView>()).CreateMapper();
            IEnumerable<UserView> userPerPages = mapper.Map<IEnumerable<UserDTO>, IEnumerable<UserView>>(db.GetPageUsers(page, pageSize));
            var pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.UsersCount()
            };
            var uvm = new UsersViewModel { PageInfo = pageInfo, Users = userPerPages };
            return View(uvm);
        }

        public ActionResult NewRegistrations(int page = 1)
        {
            int pageSize = 15; 
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, UserView>()).CreateMapper();
            IEnumerable<UserView> userPerPages = mapper.Map<IEnumerable<UserDTO>, IEnumerable<UserView>>(db.GetUnChecktedPageUser(page, pageSize));
            var pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.UsersCount()
            };
            var uvm = new UsersViewModel { PageInfo = pageInfo, Users = userPerPages };
            return View(uvm);
        }

        public ActionResult EditRole(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var user = db.GetUserById((int)id);
            if (user == null)
                return HttpNotFound();

            ViewData["RoleId"] = new SelectList(db.GetAllRoles(), "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(int id, [Bind(Include = "Id,Name,Surname,Patronymic,Email,Password,BirthDate,RoleId,SexId,Workplace,Position,Country,City,IsApproved")] UserView user)
        {
            if (id != user.Id)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                try
                {
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserView, UserDTO>()).CreateMapper();
                    var userForEdit = mapper.Map<UserView, UserDTO>(user);;
                    db.UpdateUserRole(userForEdit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.IsUserExists(user.Id))
                        return HttpNotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Registrations));
            }
            ViewData["RoleId"] = new SelectList(db.GetAllRoles(), "Id", "Name", user.RoleId);
            return View(user);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user =  db.GetUserById((int)id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
