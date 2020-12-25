using InternTask2.DAL.Services.Abstract;
using InternTask2.DAL.Services.Concrete;
using Ninject.Modules;

namespace InternTask2.BLL
{
    public class BLLNinjectModule : NinjectModule
    {
        private string connectionString;
        private string siteUrl;
        private string docLibName;
        private string listName;
        private string login;
        private string password;

        public BLLNinjectModule(string connectionString, string siteUrl, string docLibName, string listName, string login, string password)
        {
            this.connectionString = connectionString;
            this.siteUrl = siteUrl;
            this.docLibName = docLibName;
            this.listName = listName;
            this.login = login;
            this.password = password;
        }
        public override void Load()
        {
            Bind<ISPManager>().To<SPManager>()
                .WithConstructorArgument("siteUrl",siteUrl)
                .WithConstructorArgument("docLibName", docLibName)
                .WithConstructorArgument("listName", listName)
                .WithConstructorArgument("login", login)
                .WithConstructorArgument("password", password);
            Bind<ISPInitializer>().To<SPInitializer>()
                .WithConstructorArgument("siteUrl",siteUrl)
                .WithConstructorArgument("docLibName", docLibName)
                .WithConstructorArgument("listName", listName)
                .WithConstructorArgument("login", login)
                .WithConstructorArgument("password", password);
            Bind<IUnitOfWork>().To<EFUnitOfWork>()
                .WithConstructorArgument(connectionString);
        }
    }
}
