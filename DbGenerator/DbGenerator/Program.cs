using Consolas.Core;
using Consolas.Razor;
using SimpleInjector;
using System;
using System.Threading;

namespace DbGenerator
{
    public class Program : ConsoleApp<Program>
    {
        public static void Main(string[] args)
        {
            Match(args);

            Console.WriteLine("Process complete.");
            Thread.Sleep(3000);
        }

        public override void Configure(Container container)
        {
            ViewEngines.Add<RazorViewEngine>();
        }
    }
}
