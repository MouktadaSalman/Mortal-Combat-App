/* 
 * Module: DataInterfaceImpl
 * Description: The implementation of the DataInterface, has the methods to store and retrieve data from the data tier
 * Author: Jauhar, Mouktada, Ahmed
 * ID: 21494299, 20640266, 21467369
 * Version: 1.0.0.1
 */
using DataServer;
using Mortal_Combat_Data_Library;
using MortalCombatDataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    internal class DataInterfaceImpl : DataInterface
    {
        /* Class fields:
         * _playerDatabase -> retrieves the instance of the player data
         * _lobbyDatabase -> retrieves the instance of the lobby data
         * _messageDatabase -> retrieves the instance of the message data
         * _fileDatabase -> retrieves the instance of the file data
         */
        private readonly PlayerDatabase _playerDatabase = PlayerDatabase.Instance;
        private readonly LobbyDatabase _lobbyDatabase = LobbyDatabase.Instance;
        private readonly MessageDatabase _messageDatabase = MessageDatabase.Instance;
        private readonly FileDatabase _fileDatabase = FileDatabase.Instance;

        /* Method: GetNumOfPlayers
         * Description: Get the number of players in the server
         * Parameters: numOfPlayers (int)
         * Result: numOfPlayers (int)
         */
        void DataInterface.GetNumOfPlayers(out int numOfPlayers)
        {
            numOfPlayers = _playerDatabase._players.Count;
        }

        /* Method: GetNumOfLobbies
         * Description: Get the number of lobbies in the server
         * Parameters: numOfLobbies (int)
         * Result: numOfLobbies (int)
         */
        void DataInterface.GetNumOfLobbies(out int numOfLobbies)
        {
            numOfLobbies = _lobbyDatabase._lobbies.Count;
        }

        /* Method: AddPlayerToServer
         * Description: Add a player to the server
         * Parameters: player (Player)
         */
        void DataInterface.AddPlayerToServer(Player player) 
        {
            _playerDatabase._players.Add(player);
        }

        /* Method: AddPlayerToLobby
         * Description: Add a player to the selected lobby
         * Parameters: player (Player), lobbyName (string)
         */
        void DataInterface.AddPlayerToLobby(Player player, string lobbyName)
        {
            _lobbyDatabase.AddPlayerToLobby(player, lobbyName);
        }

        /* Method: AddLobbyToServer
         * Description: Add a lobby to the server
         * Parameters: lobby (Lobby)
         */
        void DataInterface.AddLobbyToServer(Lobby lobby)
        {
            _lobbyDatabase._lobbies.Add(lobby);
        }

        /* Method: CreateLobby
         * Description: Create a new lobby and add it to the server
         * Parameters: lobbyName (string)
         */
        void DataInterface.CreateLobby(string lobbyName)
        {
            Lobby lobby = new Lobby(lobbyName);
            _lobbyDatabase.AddNewLobbyToServer(lobby);
        }

        /* Method: GetPlayerForIndex
         * Description: Get the player for a specific index
         * Parameters: index (int), foundUsername (string)
         * Result: foundUsername (string)
         */
        void DataInterface.GetPlayerForIndex(int index, out string foundUsername)
        {
            foundUsername = _playerDatabase.GetUsernameByIndex(index);
        }

        /* Method: GetLobbyForIndex
         * Description: Get the lobby for a specific index
         * Parameters: index (int), foundLobbyName (string)
         * Result: foundLobbyName (string)
         */
        void DataInterface.GetLobbyForIndex(int index, out Lobby foundLobbyName)
        {
            foundLobbyName = _lobbyDatabase.GetLobbyNameByIndex(index);
        }

        /* Method: CreateMessage
         * Description: Store the new message in the database
         * Parameters: sender (string), recipient (string), content (string), messageType (int)
         */
        void DataInterface.CreateMessage(string sender, string recipent, string content, int messageType)
        {
            _messageDatabase.SaveMessage(sender, recipent, content.ToString(), messageType);
        }

        /* Method: CreateMessageF
         * Description: Store the new hyper-link message in the database. For file transfer
         * Parameters: sender (string), recipient (string), content (FileLinkBlock), messageType (int)
         */
        void DataInterface.CreateMessageF(string sender, string recipent, MessageDatabase.FileLinkBlock content, int messageType)
        {
            _messageDatabase.SaveMessage(sender, recipent, content, messageType);
        }

        /* Method: GetPlayersInLobbyCount
         * Description: Get the number of players in the lobby
         * Parameters: index (int), lobbyCount (int)
         * Result: lobbyCount (int)
         */
        void DataInterface.GetPlayersInLobbyCount(int index, out int lobbyCount)
        {
            lobbyCount = _lobbyDatabase._lobbies[index].PlayerCount;
        }

        /* Method: GetMessagesForLobby
         * Description: Get the messages for the selected lobby
         * Parameters: sender (string), lobbyName (string)
         * Result: List of messages for the lobby
         */
        List<MessageDatabase.Message> DataInterface.GetMessagesForLobby(string sender, string LobbyName)
        {
            return _messageDatabase.GetMessagesForRecipient(LobbyName); 
        }

        /* Method: GetPrivateMessages
         * Description: Get the private messages for the selected recipient
         * Parameters: sender (string), recipient (string)
         * Result: List of private messages for the recipient
         */
        List<MessageDatabase.Message> DataInterface.GetPrivateMessages(string sender,string recipent)
        {
            return _messageDatabase.GetPrivateMessagesForRecipient(sender, recipent);
        }

        /* Method: DeleteLobby
         * Description: Delete the selected lobby
         * Parameters: index (int)
         */
        void DataInterface.DeleteLobby(int index)
        {
            _lobbyDatabase._lobbies.RemoveAt(index);
        }

        /* Method: DistributeMessage
         * Description: Distribute the message to the selected recipient
         * Parameters: lobbyName (string), sender (string), recipient (string), content (object), messageType (int)
         */
        void DataInterface.DistributeMessage(string lobbyName, string sender, string recipent, object content, int messageType)
        {
            throw new NotImplementedException();
        }

        /* Method: GetAllLobbyNames
         * Description: Get all the lobby names
         * Result: List of lobby names
         */
        List<string> DataInterface.GetAllLobbyNames()
        {
            List<string> lobbyNames = new List<string>();

            foreach(Lobby l in _lobbyDatabase.GetLobbies())
            {
                lobbyNames.Add(l.LobbyName);
            }
            return lobbyNames;
        }

        /* Method: GetAllPlayersInlobby
         * Description: Get all the players in the lobby
         * Parameters: lobby (Lobby)
         * Result: List of player names
         */
        List<string> DataInterface.GetAllPlayersInlobby(Lobby lobby)
        {
            List<string> playerNames = new List<string>();

            foreach (string p in _lobbyDatabase.GetPlayersInLobby(lobby))
            {
                playerNames.Add(p);
            }
            return playerNames;
        }

        /* Method: UploadFile
         * Description: Upload the file to the server
         * Parameters: filePath (string)
         */
        void DataInterface.UploadFile(string filePath)
        {
            _fileDatabase.UploadFile(filePath);
        }

        /* Method: RetrieveFile
         * Description: Retrieve the file from the server
         * Parameters: fileName (string), fData (byte[]), fType (int)
         */
        void DataInterface.RetrieveFile(string fileName, out byte[] fData, out int fType)
        {
            _fileDatabase.RetrieveFile(fileName, out fData, out fType);
        }
    }
}
