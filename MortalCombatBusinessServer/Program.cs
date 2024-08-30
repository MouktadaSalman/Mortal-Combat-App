using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace MortalCombatBusinessServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Business Service");

            ServiceHost host;

            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();

            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(BusinessInterfaceImpl));

            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to accept on any interface. :8100 means this will use port 8100. DataService is a name for theactual service, this can be any string.
            host.AddServiceEndpoint(typeof(BusinessInterface), tcp,
           "net.tcp://0.0.0.0:8200/MortalCombatBusinessService");

            //And open the host for business!
            host.Open();

            Console.WriteLine("System Online");
            Console.ReadLine();
            //Don't forget to close the host after you're done!
            host.Close();
        }
    }
}
