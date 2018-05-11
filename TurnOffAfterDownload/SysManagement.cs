using System;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Kalikowo
{
    static class SysManagement
    {
        const string ALL_DISKS_DIRECTIVE = "_Total";

        /*
         * Shutdown PC after X seconds disk idle  ;   X = countSeconds
         */
        public static void countZeros(int countSeconds = 10)
        {
            string diskName = SelectDisk();

            PerformanceCounter performanceCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", diskName);

            int countZeros = 0; //Zeros = idle state

            Console.WriteLine("Let's begin... Reboot after: " + countSeconds + " seconds idle");

            while (true)
            {
                float nextVal = performanceCounter.NextValue(); //Get current activity value
                Console.WriteLine(nextVal);
                if (nextVal == 0)   //Check is disk idle. 
                    countZeros++;
                else                //If activity is detected
                    countZeros = 0;

                Thread.Sleep(1000);

                if (countZeros >= countSeconds) //When max seconds limit is reached, poweroff PC
                {
                    ShutDownNow();
                    break;
                }
            }
        }

        /*
         * Shutdown OS
         */
        public static void ShutDownNow()
        {
            Process.Start("ShutDown", "/s");
        }

        /*
         * Display list of categories in PerformanceCounter
         */
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

        /*
         * Get disks names (ex. C:/, D:/, etc)
         */
        private static string[] GetDisksNames()
        {
            return new PerformanceCounterCategory("PhysicalDisk").GetInstanceNames();
        }

        /*
         * Part of interface, where user decide, which disk will observe
         */
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

            if (i == 0 || i == 1)   //If somethins wrong (in case 1 disk [ex. C:], diskNames contains "C:" and "_Total" 
                return GetAllDisksString();

            Console.Write("\nYour choice: ");
            string chooseFromCw = Console.ReadKey().KeyChar.ToString(); //User inputs which disk will observe

            int choose;
            if (!Int32.TryParse(chooseFromCw, out choose))  //If wrong input (int conversion faild)
                return GetAllDisksString();

            if (choose < 0 && choose > i)   //If too small or too big value is given
                return GetAllDisksString();


            string selectedDisk = disksNames[choose];
            if (selectedDisk == ALL_DISKS_DIRECTIVE)    //If all disks selected by user, show other info
                Console.WriteLine("\n\nYour choice: all disks monitor \n");
            else
                Console.WriteLine("\n\nYour choice: " + selectedDisk + " disk \n");


            return selectedDisk;
        }

        /*
         * Method to apply all disk monitoring
         */
        private static string GetAllDisksString()
        {
            Console.WriteLine("\n\nYou choosed all disks monitoring\n");
            return ALL_DISKS_DIRECTIVE;
        }
    }
}
