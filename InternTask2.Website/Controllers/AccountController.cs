using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.BLL.Services.Concrete;
using InternTask2.Core.Models;
using InternTask2.Website.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService db;
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public AccountController(IAccountService ias)
        {
            db = ias;
        }

        public ActionResult Login()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
                var user = mapper.Map<UserDTO,User>(db.FindUser(model.Email, model.Password));
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
            ViewData["SexId"] = new SelectList(db.GetAllSexes(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new UserDTO
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
                    db.Registration(user);
                    return RedirectToAction("SuccessfulReg", "Account");
                }
                catch(ValidationException ex) 
                {
                    ModelState.AddModelError(ex.Message, "");
                }
            }
            ViewData["SexId"] = new SelectList(db.GetAllSexes(), "Id", "Name");
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
            db.Dispose();
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
