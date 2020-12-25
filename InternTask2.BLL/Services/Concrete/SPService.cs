using InternTask2.BLL.Services.Abstract;
using InternTask2.DAL.Services.Abstract;
using System;

namespace InternTask2.BLL.Services.Concrete
{
    public class SPService : ISPService
    {
        ISPInitializer spInitializer;

        public SPService(ISPInitializer spm)
        {
            spInitializer = spm;
        }
        public void Initialize()
        {
            spInitializer.Initialize();
        }
    }
}
