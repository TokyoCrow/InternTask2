using InternTask2.BLL.Services.Abstract;
using InternTask2.BLL.Services.Concrete;
using InternTask2.Core.Services.Abstract;
using InternTask2.Core.Services.Concrete;
using Ninject.Modules;

namespace InternTask2.Website
{
    public class AppNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAdminService>().To<AdminService>();
            Bind<IAccountService>().To<AccountService>();
            Bind<IUserService>().To<UserService>();
            Bind<ISendEmail>().To<InboxMailRU>();
            Bind<ISPService>().To<SPService>();
        }
    }
}