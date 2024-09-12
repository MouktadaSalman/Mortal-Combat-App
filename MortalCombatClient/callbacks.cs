using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using MortalCombatClient;
using System.Collections.Generic;
using System.Windows;

namespace MortalCombatClient
{
    public class Callbacks : PlayerCallback
    {
        private InLobbyPage _inLobbyPage;
        private Dictionary<string, PrivateMessagePage> _privateMessagePages;
        private LobbyPage _lobbyPage; 
        private MainWindow _mainWindow;

        public Callbacks()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _privateMessagePages = new Dictionary<string, PrivateMessagePage>();
        }

        public void UpdateLobbyPage(LobbyPage lobbyPage)
        {
            _lobbyPage = lobbyPage;
        }


        public void UpdateInLobbyPage(InLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;
        }

        public void UpdatePrivatePage(string chatPartner, PrivateMessagePage page)
        {
            if (page == null)
            {
                _privateMessagePages.Remove(chatPartner);
            }
            else
            {
                _privateMessagePages[chatPartner] = page;
            }
        }

        public void ReceiveLobbyMessage(string sender, string lobbyName, string content)
        {
            if (_inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.ShowMessage($"{sender}: {content}");
                });
            }
        }

        public void ReceiveLobbyMessageF(string sender, string lobbyName, MessageDatabase.FileLinkBlock content)
        {
            if (_inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.ShowLink(content);
                });
            }
        }

        public void ReceivePrivateMessage(string sender, string recipient, string content)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.HandleIncomingPrivateMessage(sender, recipient, content);
            });
        }
    }
}