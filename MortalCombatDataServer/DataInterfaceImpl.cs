using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]

    // Add the implementation of the DataInterface
    internal class interfaceImplementation : DataInterface
    {
        public void AddPlayerToLobby(string pUserName)
        {
            throw new NotImplementedException();
        }

        public void CreateLobby(string lobbyName)
        {
            throw new NotImplementedException();
        }

        public void CreateMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public void DeleteLobby(int playerCount)
        {
            throw new NotImplementedException();
        }

        public void DistributeMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayerFromLobby(string pUserName)
        {
            throw new NotImplementedException();
        }
    }
}
