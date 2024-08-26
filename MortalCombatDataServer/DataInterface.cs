using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Mortal_Combat_Data_Library;

namespace DataServer
{
    [ServiceContract]
    public interface DataInterface
    {
        [OperationContract]
        void CreateLobby(string lobbyName);

        [OperationContract]
        void AddPlayerToLobby(string pUserName);

        [OperationContract]
        void RemovePlayerFromLobby(string pUserName);

        [OperationContract]
        void CreateMessage(Message message);

        [OperationContract]

        void DistributeMessage(Message message);


        [OperationContract]
        void DeleteLobby(int playerCount);

    }
}


