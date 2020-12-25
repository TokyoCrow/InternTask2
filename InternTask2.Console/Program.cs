using InternTask2.BLL;
using InternTask2.BLL.Services.Abstract;
using InternTask2.ConsoleApp.Properties;
using Ninject;
using Ninject.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InternTask2.ConsoleApp
{
    class Program
    {
        static readonly int SleepTime = Settings.Default.SleepTimeInMinutes;
        static bool isWorking = false;
        static ISPAndDBSynchronizer synchronizer;
        static void Main(string[] args)
        {
            NinjectModule bllModule = new BLLNinjectModule(
                "DefaultConnection",
                Settings.Default.SPSiteUrl,
                Settings.Default.SPDocLibName,
                "",
                Settings.Default.SPLogin,
                Settings.Default.SPPass
                );
            IKernel kernel = new StandardKernel(new AppNinject(), bllModule);
            synchronizer = kernel.Get<ISPAndDBSynchronizer>();
            int choose;
            do
            {
                Console.Clear();
                CheckWork();
                MainMenu();
                if (int.TryParse(Console.ReadLine(), out choose))
                    switch (choose)
                    {
                        case 1:
                            {
                                if (!isWorking)
                                {
                                    isWorking = !isWorking;
                                    StartProgramm();
                                }
                                else
                                {
                                    Console.WriteLine("Programm is working");
                                }
                                break;
                            }
                        case 2:
                            {
                                isWorking = false;
                                break;
                            }
                        case 3: break;
                        default:
                            {
                                Console.WriteLine("Select an action");
                                Thread.Sleep(1000);
                                break;
                            }
                    }
            } while (choose != 3);
            Console.ReadKey();
        }

        static void CheckWork()
        {
            if (isWorking)
                Console.WriteLine("Programm is running");
            else
                Console.WriteLine("Programm is waiting");
        }

        static void MainMenu()
        {
            Console.WriteLine("1 - Start");
            Console.WriteLine("2 - Stop");
            Console.WriteLine("3 - Exit");
        }
        static async void StartProgramm()
        {
            await Task.Run(() =>
            {
                while (isWorking)
                {
                    synchronizer.SyncSPAndDB();
                    Thread.Sleep(SleepTime * 60 * 1000);
                }
            });
        }

        
    }
}
