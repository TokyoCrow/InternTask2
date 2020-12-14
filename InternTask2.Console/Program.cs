using InternTask2.ConsoleApp.Helpers;
using InternTask2.ConsoleApp.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InternTask2.ConsoleApp
{
    class Program
    {
        static readonly int SleepTime = Settings.Default.SleepTimeInMinutes;
        static bool isWorking = false;
        static void Main(string[] args)
        {
            int choose;
            do
            {
                Console.Clear();
                if (isWorking)
                    Console.WriteLine("Programm is running");
                else
                    Console.WriteLine("Programm is waiting");
                Console.WriteLine("1 - Start");
                Console.WriteLine("2 - Stop");
                Console.WriteLine("3 - Exit");
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

        static async void StartProgramm()
        {
            await Task.Run(() =>
            {
                while (isWorking)
                {
                    SyncHepler.SyncingWithSharePointNDatabase();
                    Thread.Sleep(SleepTime * 60 * 1000);
                }
            });
        }

        
    }
}
