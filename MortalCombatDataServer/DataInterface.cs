﻿/* 
 * Module: DataInterface
 * Description: An interface for the operations to store and retrieve data from the data tier 
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

namespace DataServer
{
    [ServiceContract]
    public interface DataInterface
    {
        [OperationContract]
        void GetNumOfPlayers(out int numOfPlayers);

        [OperationContract]
        void GetNumOfLobbies(out int numOfLobbies);

        [OperationContract]
        void AddPlayerToServer(Player player);

        [OperationContract]
        void AddPlayerToLobby(Player player, string lobbyName);

        [OperationContract]
        void AddLobbyToServer(string lobby);

        [OperationContract]
        void RemovePlayerFromLobby(int lobbyIndex, int playerIndex);

        [OperationContract]
        void RemovePlayerFromServer(int index);

        [OperationContract]
        void GetPlayerForIndex(int index, out Player foundUsername);

        [OperationContract]
        void GetPlayerInLobbyForIndex(int playerIndex, int lobbyIndex, out Player foundPlayer);

        [OperationContract]
        void GetLobbyForIndex(int index, out Lobby foundLobbyName);

        [OperationContract]
        void CreateMessage(string sender, string recipent, string content, int messageType, DateTime dateTime);

        [OperationContract]
        void CreateMessageF(string sender, string recipent, MessageDatabase.FileLinkBlock content, int messageType, DateTime dateTime);


        [OperationContract]
        List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent);

        [OperationContract]
        List<MessageDatabase.Message> GetMessagesForLobby(string sender, string lobbyName);

        [OperationContract]
        List<string> GetAllLobbyNames();

        [OperationContract]
        void DeleteLobby(int index);

        [OperationContract]
        List<string> GetAllPlayersInlobby(string lobby);

        [OperationContract]
        void UploadFile(string filePath);

        [OperationContract]
        void RetrieveFile(string fileName, out byte[] fData, out int fType);
    }
}


