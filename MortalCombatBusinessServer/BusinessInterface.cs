﻿/* 
 * Module: BusinessInterface
 * Description: The business logic for the Mortal Combat game
 * Author: Jauhar, Mouktada, Ahmed
 * ID: 21494299, 20640266, 21467369
 * Version: 1.0.0.1
 */
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
        [OperationContract]
        void AddPlayerToServer(Player player);

        [OperationContract]
        void AddLobbyToServer(Lobby lobby);

        [OperationContract]
        void AddPlayertoLobby(Player player, string lobbyName);

        [OperationContract]        
        void SendPrivateMessage(string sender, string recipent, string content);

        [OperationContract]
        List<MessageDatabase.Message> GetPrivateMessages(string user1, string user2);

        [OperationContract]
        void NotifyPrivatePlayer(string sender, string recipent, string content);

        [OperationContract]
        void DistributeMessageToLobby(string lobbyName, string sender, string content);

        [OperationContract]
        void DistributeMessageToLobbyF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content);

        [OperationContract]
        List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent);

        [OperationContract]
        void NotifyDistributedMessages(string lobbyName, string sender, string content);

        [OperationContract]
        void NotifyDistributedMessagesF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content);

        [OperationContract]
        List<string> GetAllLobbyNames();

        [OperationContract]
        List<string> GetPlayersInLobby(Lobby lobby);

        [OperationContract]
        void CheckUsernameValidity(string username, out bool isValid);

        [OperationContract]
        void CheckLobbyNameValidity(string lobbyName, out bool isValid);

        [OperationContract]
        void DeleteLobby(string lobbyName);

        [OperationContract]
        void UploadFile(string filePath);

        [OperationContract]
        void DownloadFile(string filePath);
    }

    public interface PlayerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceivePrivateMessage(string sender, string lobbyName, string content);

        [OperationContract(IsOneWay = true)]
        void ReceiveLobbyMessage(string sender, string lobbyName, string content);

        [OperationContract(IsOneWay = true)]
        void ReceiveLobbyMessageF(string sender, string lobbyName, MessageDatabase.FileLinkBlock content);
    }
}
