using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;
using System.Reflection;

namespace MortalCombatBusinessServer
{
    internal class Program
    {
        private static string downloadFile;
        private static ServiceHost host;
        static void Main(string[] args)
        {
            Console.WriteLine("Business Service");

            //The location of the download file of all the files
            downloadFile = @"" + Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName,
                                        "MortalCombatDownloads");

            //If the app downloads folder doesn't exist yet... create it
            if (!Directory.Exists(downloadFile)) { Directory.CreateDirectory(downloadFile); }

            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();

            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(BusinessInterfaceImpl));

            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to accept on any interface. :8100 means this will use port 8100. DataService is a name for theactual service, this can be any string.
            host.AddServiceEndpoint(typeof(BusinessInterface), tcp,
           "net.tcp://0.0.0.0:8200/MortalCombatBusinessService");

            //And open the host for business!
            host.Open();

            Console.WriteLine("System Online");
            Console.ReadLine();

            //Call event handlers when the server is being closed:
            //Handle Ctrl+C or Ctrl+Break
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);

            //Handle when the process is about to exit (including window close)
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            //Don't forget to close the host after you're done!
            host.Close();
        }

        /* Method: OnCancelKeyPress
         * Description: An event handler when the server is being closed 
         *              (when Ctrl+C/Break typed in terminal)
         * Paramters: sender (object), e (ConsoleCancelEventArgs)
         */
        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            //Check if it exists
            if (Directory.Exists(downloadFile))
            {
                try
                {
                    //Delete the directory
                    Directory.Delete(downloadFile);
                    Console.WriteLine("Deleting the downloaded files stash...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error... \n{ ex.ToString()}");
                }
            }
            else
            {
                Console.WriteLine($"The local downloads folder stash doesn't exist (never created)");
            }

        }

        /* Method: OnProcessExit
         * Description: An event handler when the server is being closed 
         *              (window close/ other shutdowns)
         * Paramters: sender (object), e (EventArgs)
         */
        static void OnProcessExit(object sender, EventArgs e)
        {
            //Check if it exists
            if (Directory.Exists(downloadFile))
            {
                try
                {
                    //Delete the directory
                    Directory.Delete(downloadFile);
                    Console.WriteLine("Deleting the downloaded files stash...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error... \n{ex.ToString()}");
                }
            }
            else
            {
                Console.WriteLine($"The local downloads folder stash doesn't exist (never created)");
            }
        }

        /* Method: Cleanup
         * Description: The actual cleanup when server closes
         * Parameters: none
         */
        static void Cleanup()
        {
            //Close the host if it's open
            if (host != null && host.State == CommunicationState.Opened)
            {
                try
                {
                    host.Close();
                    Console.WriteLine("Service host closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing service host: {ex.Message}");
                }
            }

            // Check if the directory exists and delete it
            if (Directory.Exists(downloadFile))
            {
                Console.WriteLine("Attempting to delete the directory...");
                try
                {
                    Directory.Delete(downloadFile, true); // 'true' ensures recursive deletion
                    Console.WriteLine("Directory deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting directory: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("The local downloads folder stash doesn't exist (never created).");
            }
        }
    }
}
