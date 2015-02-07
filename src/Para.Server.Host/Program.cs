using System;
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceProcess;

using Para.Server.Host.Configuration;

namespace Para.Server.Host
{
    class Program
    {
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                PrepareConsole();

                Console.WriteLine("setcrm para service is starting...");

                IdentifyConnectingDB();

                Bootstrapper.Initialize();

                Console.WriteLine("service started!");
                Console.WriteLine("");
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new ParaWindowsService() });
            }
        }

        private static void IdentifyConnectingDB()
        {
            var connectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Para"].ConnectionString);

            Console.WriteLine(string.Empty);
            Console.WriteLine("connecting to {0} database", connectionString.InitialCatalog);

            using (var cnn = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    cnn.Open();
                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR!");
                    Console.WriteLine("couldn't connect to {0}", connectionString.InitialCatalog);
                    Console.WriteLine(string.Empty);
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        private static void PrepareConsole()
        {
            Console.Title = "Para Service / SetCrm / Argeset";
            try
            {
                Console.SetWindowSize(75, 15);
                Console.SetWindowPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Clear();
            }
            catch
            { }
        }
    }
}
