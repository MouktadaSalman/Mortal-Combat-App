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
        void GetLobbyForIndex(int index, out string foundLobbyName);

        //[OperationContract]
        //void DeleteLobby(string lobbyName, Lobby lobbyToDelete);

        [OperationContract]
        void CreateMessage(string sender, string recipent, object content, int messageType);

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
        void DownloadFile(string fileName);
    }
}


