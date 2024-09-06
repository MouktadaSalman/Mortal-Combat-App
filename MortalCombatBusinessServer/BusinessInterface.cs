using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Mortal_Combat_Data_Library;

namespace MortalCombatBusinessServer
{
    [ServiceContract(CallbackContract = typeof(PlayerCallback))]
    public interface BusinessInterface
    {
        //[OperationContract]
        //void DeleteLobby(string lobbyName);

        [OperationContract]
        void AddPlayerToServer(Player player);

        [OperationContract]
        void AddLobbyToServer(Lobby lobby);

        [OperationContract]

        void AddPlayertoLobby(Player player, string lobbyName);

        //[OperationContract]
        //void RemovePlayerFromServer(string pUserName);


        //Private Messaging methods so far..
        [OperationContract]
        
        void SendPrivateMessage(string sender, string recipent, object content);

      
     
        [OperationContract]
        List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent);


        [OperationContract]
        void NotifyPrivatePlayer(string sender, string recipent, string content);



        //lobby Messaging methods so far..
        [OperationContract]
        void DistributeMessageToLobby(string lobbyName, string sender, object content);


        [OperationContract]
        List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent);


        [OperationContract]
        void NotifyDistributedMessages(string lobbyName, string sender, string content);


        [OperationContract]
        List<string> GetAllLobbyNames();

        [OperationContract]

        List<string> GetPlayersInLobby(Lobby lobby);

        [OperationContract]
        void CheckUsernameValidity(string username, out bool isValid);

        [OperationContract]
        void CheckLobbyNameValidity(string lobbyName, out bool isValid);

        [OperationContract]
        void DeleteLobby(string lobbyName, out bool doesHavePlayers);

        [OperationContract]
        void UploadFile(string filePath);

        [OperationContract]
        void DownloadFile(string fileName);

    }

    public interface PlayerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceivePrivateMessage(string sender, string lobbyName, string content);

        [OperationContract(IsOneWay = true)]
        void ReceiveLobbyMessage(string sender, string lobbyName, string content);
    }
}
