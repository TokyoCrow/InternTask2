using InternTask2.Website.Services.Abstract;
using InternTask2.Website.Services.Concrete;
using Ninject.Modules;

namespace InternTask2.Website
{
    public class AppNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISharePointManager>().To<SharePointManager>();
            Bind<ISendEmail>().To<InboxMailRU>();
        }
    }
}