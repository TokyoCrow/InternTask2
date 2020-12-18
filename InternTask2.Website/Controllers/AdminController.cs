using InternTask2.Website.Helpers;
using InternTask2.Website.Models;
using InternTask2.Website.Services.Abstract;
using InternTask2.Website.Services.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationContext db;
        private ISendEmail mail;
        private ISharePointManager spManager;

        public AdminController(ISendEmail _mail, ISharePointManager _spManager)
        {
            db = new ApplicationContext();
            mail = _mail;
            spManager = _spManager;
        }

        public async Task<ActionResult> Approve(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound();

            string password = PasswordHelper.GetRandomPassword();
            if (await mail.Send($"Your password: {password}", user.Email, "You were approved!"))
            {
                user.Password = PasswordHelper.GetHashedPassword(user.Email, password);
                user.IsApproved = true;
                spManager.ApproveUser(user);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            else
                return RedirectToAction("EmailSendingError");

            return RedirectToAction("NewRegistrations");
        }

        public async Task<ActionResult> Reject(int id)
        {
            var user = await db.Users.FindAsync(id); 
            if (user == null)
                return HttpNotFound();

            string password = PasswordHelper.GetRandomPassword();
            if (await mail.Send($"Sorry.", user.Email, "You were rejected!"))
            {
                spManager.RejectUser(user);
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
            else
                return RedirectToAction("EmailSendingError");

            return RedirectToAction("NewRegistrations");
        }

        public ActionResult Registrations(int page = 1)
        {
            int pageSize = 15;
            IEnumerable<User> userPerPages = db.Users
                .Include(u => u.Role)
                .Include(u => u.Sex)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            var pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.Users.Count()
            };
            var uvm = new UsersViewModel { PageInfo = pageInfo, Users = userPerPages };
            return View(uvm);
        }

        public ActionResult NewRegistrations(int page = 1)
        {
            int pageSize = 15;
            IEnumerable<User> userPerPages = db.Users
                .Where(u => u.IsApproved == false)
                .Include(u => u.Role)
                .Include(u => u.Sex)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            var pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.Users.Count()
            };
            var uvm = new UsersViewModel { PageInfo = pageInfo, Users = userPerPages };
            return View(uvm);
        }

        public async Task<ActionResult> EditRole(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await db.Users
                .Include(u => u.Sex)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return HttpNotFound(); 

            ViewData["RoleId"] = new SelectList(db.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(int id, [Bind(Include = "Id,Name,Surname,Patronymic,Email,Password,BirthDate,RoleId,SexId,Workplace,Position,Country,City,IsApproved")] User user)
        {
            if (id != user.Id)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                        return HttpNotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Registrations));
            }
            ViewData["RoleId"] = new SelectList(db.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await db.Users
                .Include(u => u.Role)
                .Include(u => u.Sex)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [NonAction]
        private bool UserExists(int id) 
            => db.Users.Any(e => e.Id == id);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
