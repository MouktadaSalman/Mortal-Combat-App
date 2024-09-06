using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MortalCombatClient
{

    //this is the implemntation for the rest of methods in BusinessInterface, under PlayerCallBack interface
    public class callbacks : PlayerCallback
    {
        private inLobbyPage _inLobbyPage;
        private privateMessagePage _privateMessagePage;
        private bool _isLobbyPageActive;

        public callbacks(inLobbyPage nInLobbyPage = null, privateMessagePage privateMessagePage = null)
        {
            _inLobbyPage = nInLobbyPage;
            _privateMessagePage = privateMessagePage;
        }

        public void UpdateLobbyPage(inLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;
            _isLobbyPageActive = true; 
        }

        public void UpdatePrivatePage(privateMessagePage privateMessagePage)
        {
            _privateMessagePage = privateMessagePage;
            _isLobbyPageActive = false;
        }

        public void ReceiveLobbyMessage(string sender, string lobbyName, string content)
        {
            if (_isLobbyPageActive && _inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.showMessage($"{sender}: {content}");
                });
            }
        }

        public void ReceiveLobbyMessageF(string sender, string lobbyName, MessageDatabase.FileLinkBlock content)
        {
            if (_isLobbyPageActive && _inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.showLink(content);
                });
            }
        }

        public void ReceivePrivateMessage(string sender, string recipient, string content)
        {
            if (!_isLobbyPageActive && _privateMessagePage != null)
            {
                _privateMessagePage.Dispatcher.Invoke(() =>
                {
                    _privateMessagePage.showMessage($"{sender}: {content}");
                });
            }
        }
    }
}
