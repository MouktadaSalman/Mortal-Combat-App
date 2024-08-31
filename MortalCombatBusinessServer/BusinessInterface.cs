using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Mortal_Combat_Data_Library;

namespace MortalCombatBusinessServer
{
    [ServiceContract(CallbackContract =typeof(PlayerCallback))]
    public interface BusinessInterface
    {
        [OperationContract]
        void CreateLobby(string lobbyName);

        [OperationContract]
        void AddPlayerToServer(string pUserName);

        [OperationContract]
        void RemovePlayerFromServer(string pUserName);

        [OperationContract]
        void CreateMessage(string sender, string recipent, object content, int messageType, DateTime dateTime);

        [OperationContract]
        void DistributeMessage(Message message);

        [OperationContract]
        void DeleteLobby(string lobbyName);

        [OperationContract]
        Player GetPlayerUsingUsername(string username);

        [OperationContract]
        Lobby GetLobbyUsingName(string lobbyName);

        [OperationContract]

        List<Lobby> GetAllLobbies();

    }


    public interface PlayerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceivePrivateMessage(Message message);

        [OperationContract(IsOneWay = true)]
        void ReceiveLobbyMessage(Message message);
    }

}
