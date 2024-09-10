/* 
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
        void AddLobbyToServer(Lobby lobby);

        [OperationContract]
        void CreateLobby(string lobbyName);

        [OperationContract]
        void GetPlayerForIndex(int index, out string foundUsername);

        [OperationContract]
        void GetLobbyForIndex(int index, out Lobby foundLobbyName);

        [OperationContract]
        void CreateMessage(string sender, string recipent, string content, int messageType);

        [OperationContract]
        void CreateMessageF(string sender, string recipent, MessageDatabase.FileLinkBlock content, int messageType);

        [OperationContract]
        void DistributeMessage(string lobbyName, string sender, string recipent, object content, int messageType);

        [OperationContract]
        List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent);

        [OperationContract]
        List<MessageDatabase.Message> GetMessagesForLobby(string sender, string lobbyName);

        [OperationContract]
        List<string> GetAllLobbyNames();

        [OperationContract]
        void GetPlayersInLobbyCount(int index, out int lobbyCount);

        [OperationContract]
        void DeleteLobby(int index);

        [OperationContract]
        List<string> GetAllPlayersInlobby(Lobby lobby);

        [OperationContract]
        void UploadFile(string filePath);

        [OperationContract]
        void RetrieveFile(string fileName, out byte[] fData, out int fType);
    }
}


