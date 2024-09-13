/* 
 * Module: lobbyPage
 * Description: This module is responsible for the lobby functionality of the game. It allows players to create, join, and leave lobbies.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, 21494299
 * Version: 1.0.0.2
 */
using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using MortalCombatClient;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace MortalCombatClient
{


    //This class implements the PlayerCallback interface.
    //It's used by the client to handle incoming messages from the server for lobby and private messages.
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class Callbacks : PlayerCallback
    {
        /* Class fields:
         * _inLobbyPage -> the calling lobby page creating the pull request
         * _privateMessagePages -> contains all the private message pages linked to the calling lobby
         * _lobbyPage -> the calling main lobby page creating the pull request
         * _mainWindow -> the calling main window creating the pull request
         */
        private InLobbyPage _inLobbyPage;
        private Dictionary<string, PrivateMessagePage> _privateMessagePages;
        private LobbyPage _lobbyPage;
        private MainWindow _mainWindow;

        public Callbacks()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _privateMessagePages = new Dictionary<string, PrivateMessagePage>();
        }

        /* Method: UpdateLobbyPage
         * Description: To set where the callbacks go/the recipient of pull requests 
         *              (any instance of the main lobby page)
         * Parameters: lobbyPage (LobbyPage)
         */
        public void UpdateLobbyPage(LobbyPage lobbyPage)
        {
            _lobbyPage = lobbyPage;
        }

        /* Method: UpdateInLobbyPage
         * Description: To set where the callbacks go/the recipient of pull requests 
         *              (any instance of an inside lobby page)
         * Parameters: lobbyPage (InLobbyPage)
         */
        public void UpdateInLobbyPage(InLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;
        }

        /* Method: UpdatePrivatePage
         * Description: To set where the callbacks go/the recipient of pull requests 
         *              (any instance of a private messaging page)
         * Parameters: chatPartner (string), page (PrivateMessagePage)
         */
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

        /* Method: ReceiveLobbyMessage
         * Description: To send the string messages for the pull requesting lobby page
         * Parameters: sender (string), lobbyName (string), content (string)
         */
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

        /* Method: ReceiveLobbyMessageF
         * Description: To send the file hyperlink messages for the pull requesting lobby page
         * Parameters: sender (string), lobbyName (string), content (FileLinkeBlock)
         */
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

        /* Method: ReceivePrivateMessage
         * Description: To send the string messages for the pull requesting private
         *              messsaging page
         * Parameters: sender (string), lobbyName (string), content (string)
         */
        public void ReceivePrivateMessage(string sender, string recipient, string content)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.HandleIncomingPrivateMessage(sender, recipient, content);
            });
        }

        /* Method: NotifyLobbyListUpdate
         * Description: To update the lobby list for the pull requesting main lobby page
         * Parameters: sender (string), lobbyName (string), content (string)
         */
        public void NotifyLobbyListUpdate()
        {
            if (_lobbyPage != null)
            {
                _lobbyPage.Dispatcher.Invoke(() => _lobbyPage.RefreshLists());
            }
        }
    }
}