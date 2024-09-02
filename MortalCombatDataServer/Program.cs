using DataServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataServer
{
    internal class Program
    {
        static void Main(string[] args)
        {


            //This should *definitely* be more descriptive.
            Console.WriteLine("Data Service");

            //This is the actual host service system
            ServiceHost host;

            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();

            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(DataInterfaceImpl));

            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to accept on any interface. :8100 means this will use port 8100. DataService is a name for theactual service, this can be any string.
            host.AddServiceEndpoint(typeof(DataInterface), tcp,
           "net.tcp://0.0.0.0:8100/MortalCombatDataService");


            //And open the host for business!
            host.Open();

            Console.WriteLine("System Online");
            Console.ReadLine();
            //Don't forget to close the host after you're done!
            host.Close();
        }
    }
}
