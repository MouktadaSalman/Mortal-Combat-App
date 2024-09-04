﻿using System;
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
        //[OperationContract]
        //void DeleteLobby(string lobbyName);

        [OperationContract]
        void AddPlayerToServer(Player player);

        [OperationContract]
        void AddLobbyToServer(Lobby lobby);

        //[OperationContract]
        //void RemovePlayerFromServer(string pUserName);

        [OperationContract]
        void SendPrivateMessage(string sender, string recipent, object content);

        [OperationContract]
        List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent);


        [OperationContract]
        List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent);

        [OperationContract]
        void DistributeMessageToLobby(string lobbyName, string sender, object content);

        

        [OperationContract]
        List<string> GetAllLobbyNames();

        [OperationContract]
        void CheckUsernameValidity(string username, out bool isValid);

        [OperationContract]
        void CheckLobbyNameValidity(string lobbyName, out bool isValid);

        [OperationContract]
        void DeleteLobby(string lobbyName, out bool doesHavePlayers);

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
