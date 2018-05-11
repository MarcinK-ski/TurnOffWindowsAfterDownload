using System;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Kalikowo
{
    static class SysManagement
    {
        public static void countZeros(int zeroLimit = 10)
        {
            string diskName = SelectDisk();

            PerformanceCounter performanceCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", diskName);

            int countZeros = 0;

            Console.WriteLine("Let's begin... Reboot after: " + zeroLimit + " seconds");

            while (true)
            {
                float nextVal = performanceCounter.NextValue();
                Console.WriteLine(nextVal);
                if (nextVal == 0)
                    countZeros++;
                else
                    countZeros = 0;

                Thread.Sleep(1000);

                if (countZeros >= zeroLimit)
                {
                    RebootingNow();
                    break;
                }
            }
        }

        public static void RebootingNow()
        {
            Process.Start("ShutDown", "/s");
        }

        public static void ListCategories()
        {
            foreach (var category in PerformanceCounterCategory.GetCategories())
            {
                var txt = new StringBuilder("Nazwa: " + category.CategoryName);
                txt.Append("\n Help: " + category.CategoryHelp + "\n\n\n");
                Console.WriteLine(txt);
            }

            Console.WriteLine("\n\n\n\n\n\n");


            Console.WriteLine("Press Enter to continue...");
            Console.ReadKey();
        }

        private static string[] GetDisksNames()
        {
            return new PerformanceCounterCategory("PhysicalDisk").GetInstanceNames();
        }

        public static string SelectDisk()
        {
            int i = 0;

            string[] disksNames = GetDisksNames();

            foreach (var disk in disksNames)
            {
                string[] diskName = disk.Split(' ');    //We gets ex "1. Q:", so to get correct disk name => split => select last element of splitted array

                if (i == 0)
                    Console.WriteLine("Select disks from below list:");

                StringBuilder result = new StringBuilder();

                if (disk == "_Total")
                    result.Append("Type no.: " + i++ + " to get all disks");
                else
                    result.Append("Type no.: " + i++ + " to get disk: " + diskName[diskName.Length - 1]);

                Console.WriteLine(result);
            }

            if (i == 0)
                return GetAllDisksString();

            Console.Write("\nYour choice: ");
            string chooseFromCw = Console.ReadKey().KeyChar.ToString();
            int choose;
            if (!Int32.TryParse(chooseFromCw, out choose))
                return GetAllDisksString();

            if (choose < 0 && choose > i)
                return GetAllDisksString();


            string selectedDisk = disksNames[choose];
            Console.WriteLine("\n\nYour choice: " + selectedDisk + " disk \n");


            return selectedDisk;
        }

        private static string GetAllDisksString()
        {
            Console.WriteLine("\n\nYou choosed all disks monitoring\n");
            return "_Total";
        }
    }
}
