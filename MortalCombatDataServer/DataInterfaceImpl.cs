﻿using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]

    // Add the implementation of the DataInterface
    internal class DataInterfaceImpl : DataInterface
    {

        private readonly PlayerDatabase _playerDatabase = PlayerDatabase.Instance;
        private readonly LobbyDatabase _lobbyDatabase = LobbyDatabase.Instance;
        // to add a player to the selected existing lobby

        void DataInterface.GetNumOfPlayers(out int numOfPlayers)
        {
            numOfPlayers = _playerDatabase._players.Count;
        }

        void DataInterface.GetNumOfLobbies(out int numOfLobbies)
        {
            numOfLobbies = _lobbyDatabase._lobbies.Count;
        }

        void DataInterface.AddPlayerToServer(Player player) 
        {
            _playerDatabase._players.Add(player);
        }

        void DataInterface.AddLobbyToServer(Lobby lobby)
        {
            _lobbyDatabase._lobbies.Add(lobby);
        }

        void DataInterface.CreateLobby(string lobbyName)
        {
            Lobby lobby = new Lobby(lobbyName);
            _lobbyDatabase.AddNewLobbyToServer(lobby);
        }

        void DataInterface.GetPlayerForIndex(int index, out string foundUsername)
        {
            foundUsername = _playerDatabase.GetUsernameByIndex(index);
        }
        
        void DataInterface.GetLobbyForIndex(int index, out string foundLobbyName)
        {
            foundLobbyName = _lobbyDatabase.GetLobbyNameByIndex(index);
        }

        void DataInterface.CreateMessage(string sender, string recipent, object content, int messageType, DateTime dateTime)
        {
            Message message = new Message(sender, recipent, content, messageType, dateTime);    
            
        }

        void DataInterface.GetPlayersInLobbyCount(int index, out int lobbyCount)
        {
            lobbyCount = _lobbyDatabase._lobbies[index].PlayerCount;
        }
        
        void DataInterface.DeleteLobby(int index)
        {
            _lobbyDatabase._lobbies.RemoveAt(index);
        }

        //void DataInterface.DeleteLobby(string lobbyName, Lobby lobbyToDelete)
        //{
        //    _lobbyDatabase.RemoveLobby(lobbyToDelete);
        //}

        void DataInterface.DistributeMessage(string lobbyName, string sender, string recipent, object content, int messageType, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        //void DataInterface.RemovePlayerFromServer(string pUserName, Player playerToRemove)
        //{
        //    _playerDatabase.RemovePlayerFromServer(playerToRemove);
        //}

        List<string> DataInterface.GetAllLobbyNames()
        {
            List<string> lobbyNames = new List<string>();

            foreach(Lobby l in _lobbyDatabase.GetLobbies())
            {
                lobbyNames.Add(l.LobbyName);
            }

            return lobbyNames;
        }
    }
}
