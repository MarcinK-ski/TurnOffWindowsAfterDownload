using System;
using Kalikowo;
using System.Threading;
using System.Diagnostics;

namespace TurnOffAfterDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Shutdown after[seconds; min 10 sec]: ");
            string maxSecondsString = Console.ReadLine().ToString();
            int maxSeconds;
            if (Int32.TryParse(maxSecondsString, out maxSeconds) && maxSeconds >= 10)
                SysManagement.countZeros(maxSeconds);
            else
            {
                Console.WriteLine("Wrong input. Press Enter to exit...");
                Console.ReadKey();
            }
        }
    }
}
