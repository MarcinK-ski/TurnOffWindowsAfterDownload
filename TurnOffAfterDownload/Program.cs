using System;
using Kalikowo;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TurnOffAfterDownload
{
    class Program
    {
        const string typicalRemoteCommand = "init 0";   //Default command to remote shutdown (if no user password given)
        static string password; //User defined password (optional)

        static void Main(string[] args)
        {
            Console.WriteLine("What would You do:\n\ta) Shutdown PC after X seconds\n\tb) Shutdown PC after remote device command ");
            char choose = Console.ReadKey().KeyChar; Console.WriteLine('\n');

            if (choose == 'a')
                ShutdownWithTime();
            else if (choose == 'b')
                ShutdownWithRemoteDevice();
            else
                Console.WriteLine("BLEDNA OPCJA");

        }

        //Method to turnoff PC after X seconds idle of selected disk(s)
        static void ShutdownWithTime()
        {
            Console.Write("Shutdown after[seconds; min 10 sec]: ");
            string maxSecondsString = Console.ReadLine().ToString();
            int maxSeconds;
            if (Int32.TryParse(maxSecondsString, out maxSeconds) && maxSeconds >= 10)
                SysManagement.countZeros(maxSeconds);   //Start monitoring method
            else
            {
                Console.WriteLine("Wrong input. Press Enter to exit...");
                Console.ReadKey();
            }
        }

        //Method to turnoff PC when correct pass/command is given via telnet
        static void ShutdownWithRemoteDevice()
        {
            Console.WriteLine("Enter IP yours remote device or leave empty to using any device: "); //Primitive protection from unauthorized connection (optional)
            string ip = Console.ReadLine();
            bool anyIp = false;
            if (ip == "" || ip == null)
                anyIp = true;

            Console.WriteLine("You can give special password to shutdown PC (leave empty if nope): "); //Replace default command by custom password
            password = Console.ReadLine();

            TcpListener tcpListener = new TcpListener(IPAddress.Any, 12345);
            tcpListener.Start();

            //At the end of loop, disconnect client to slow down brute force method in "password case"
            while(true)
            {
                Console.WriteLine("Waiting for connection request...");
                Socket socket = tcpListener.AcceptSocket();
                IPEndPoint clientIP = socket.RemoteEndPoint as IPEndPoint;

                if(clientIP.Address.ToString() == ip || anyIp)
                {
                    Console.WriteLine($"{clientIP.Address} is connected");

                    using (NetworkStream netStream = new NetworkStream(socket))
                    {
                        using (StreamReader streamReader = new StreamReader(netStream))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(new NetworkStream(socket)))
                            {
                                streamWriter.WriteLine("You've connected to TOAD system, to turn off, give your pass or if it wasnt given, type \"init 0\"");
                            }

                            string msg = streamReader.ReadLine();

                            if (isPermitedToTurnOff(msg))
                            {
                                Console.WriteLine("DOWN!");
                                SysManagement.ShutDownNow();
                                break;
                            }
                            else
                                Console.WriteLine("WRONG command");
                        }
                    }
                }
                else
                    Console.WriteLine($"{clientIP.Address} is not allowed!");

                Console.WriteLine($"{clientIP.Address} was disconnected");
                socket.Disconnect(true);
            }
        }

        static bool isPermitedToTurnOff(string msg)
        {
            if (password == null || password == "")
            {
                if (msg.ToLower() == typicalRemoteCommand)
                    return true;
            }
            else
            {
                if (password == msg)
                    return true;
            }

            return false;
        }
    }
}
