using InternTask2.BLL.Services.Abstract;
using InternTask2.BLL.Services.Concrete;
using Ninject.Modules;

namespace InternTask2.ConsoleApp
{
    public class AppNinject : NinjectModule
    {
        public override void Load()
        {
            Bind<ISPAndDBSynchronizer>().To<SPAndDBSynchronizer>();
        }
    }
}
