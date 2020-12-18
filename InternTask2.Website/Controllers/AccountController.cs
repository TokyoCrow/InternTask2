using InternTask2.Website.Helpers;
using InternTask2.Website.Models;
using InternTask2.Website.Services.Abstract;
using Microsoft.Owin.Security;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        private ISharePointManager spManager;
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public AccountController(ISharePointManager _spManager)
        {
            db = new ApplicationContext();
            spManager = _spManager;
        }

        public ActionResult Login()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = PasswordHelper.GetHashedPassword(model.Email, model.Password);
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == hashedPassword);
                if (user != null)
                {
                    Authenticate(user);

                    if (user.Role.Name == "admin")
                        return RedirectToAction("NewRegistrations", "Admin");
                    if (user.Role.Name == "user")
                        return RedirectToAction("Index", "User");
                }
                else
                    ModelState.AddModelError("", "Invalid email and(or) password");
            }
            return View(model);
        }

        private void Authenticate(User user)
        {
            var claim = new ClaimsIdentity("ApplicationCookie",
                                           ClaimsIdentity.DefaultNameClaimType,
                                           ClaimsIdentity.DefaultRoleClaimType);

            claim.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email, ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
            claim.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name, ClaimValueTypes.String));

            claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                     "OWIN Provider",
                                     ClaimValueTypes.String));

            var id = new ClaimsIdentity("ApplicationCookie",
                                        ClaimsIdentity.DefaultNameClaimType,
                                        ClaimsIdentity.DefaultRoleClaimType);
            AuthenticationManager.SignOut();
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, claim);
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewData["SexId"] = new SelectList(db.Sexes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        Patronymic = model.Patronymic,
                        BirthDate = model.BirthDate,
                        SexId = model.SexId,
                        Workplace = model.Workplace,
                        Position = model.Position,
                        Country = model.Country,
                        City = model.City
                    };
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;
                    Sex userSex = await db.Sexes.FirstOrDefaultAsync(s => s.Id == model.SexId);
                    if (userSex != null)
                        user.Sex = userSex;
                    user.SPId = spManager.AddUserToCustomList(user);
                    if (user.SPId > 0)
                    {
                        db.Users.Add(user);
                        await db.SaveChangesAsync();
                        return RedirectToAction("SuccessfulReg", "Account");
                    }
                }
                ModelState.AddModelError("", "Email already used");
            }
            ViewData["SexId"] = new SelectList(db.Sexes, "Id", "Name");
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult SuccessfulReg()
            => View();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
