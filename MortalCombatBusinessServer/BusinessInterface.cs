using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Mortal_Combat_Data_Library;

namespace MortalCombatBusinessServer
{
    [ServiceContract]
    //[ServiceContract(CallbackContract =typeof(PlayerCallback))]
    public interface BusinessInterface
    {
        [OperationContract]
        void CreateLobby(string lobbyName);

        [OperationContract]
        void DeleteLobby(string lobbyName);

        [OperationContract]
        void AddPlayerToLobby(string lobbyName, string userName);

        [OperationContract]
        void AddPlayerToServer(string pUserName);

        [OperationContract]
        void RemovePlayerFromServer(string pUserName);

        [OperationContract]
        void CreateMessage(string sender, string recipent, object content, int messageType, DateTime dateTime);

        [OperationContract]
        void DistributeMessage(string lobbyName, string sender, string recipent, object content, int messageType, DateTime dateTime);


        [OperationContract]
        Player GetPlayerUsingUsername(string username);

        [OperationContract]
        Lobby GetLobbyUsingName(string lobbyName);

        [OperationContract]
        List<Lobby> GetAllLobbies();

    }


    /*
     * This interface has 2 operations for getting the messsages that were send from the above operations
     */
    public interface PlayerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceivePrivateMessage(Message message);

        [OperationContract(IsOneWay = true)]
        void ReceiveLobbyMessage(Message message);
    }

}
