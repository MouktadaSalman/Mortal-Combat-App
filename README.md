## Project Overview

This project is an implementation of an online gaming lobby for Mortal Kombat X using .NET Windows Communication Foundation (WCF) and a Windows Presentation Foundation (WPF) client. The system allows multiple players to connect to a server, join lobby rooms, exchange messages, send private messages, and share files.

## Features

**Gaming Lobby Server**

- User Management: Ensures unique player logins and prevents duplicate usernames.

- Lobby Room Management: Allows players to create, join, and leave lobby rooms.

- Message Distribution: Sends messages from a player to all members in the same lobby.

- Private Messaging: Enables private communication between players.

- File Sharing: Facilitates image and text file sharing among players in a lobby room.

**Client Application (WPF)**

- User Login: Players log in with a unique username to access the system.

- Lobby Room Selection: Displays available lobby rooms for players to join.

- Lobby Room Creation: Allows users to create new lobby rooms with unique names.

- Lobby Room Messaging: Players can send and receive messages within a room.

- Private Messaging: Enables one-on-one private communication.

- File Sharing: Allows users to upload and download images and text files.

- Logout: Players can log out when they are finished.

## Additional Features

Multithreading:

Separate threads update lobby room lists and available players.

Message retrieval and private messages operate in separate threads.

File upload/download operations run on dedicated threads with a progress bar.

## Technologies Used

.NET WCF – Server implementation for handling player connections and messaging.

WPF (Windows Presentation Foundation) – Client GUI application.

C# – Programming language used for both server and client.

Multithreading – Used to make the system more responsive and closer to real-time.

## System Requirements

Windows OS

.NET Framework

Visual Studio (for development and testing)

Local static IP and port configuration for the server

## Setup Instructions

Clone or download the project repository.

Open the solution in Visual Studio.

Start the Gaming Lobby Server project.

Run multiple instances of the Client Application to simulate players.

Test features like login, room creation, messaging, and file sharing.

## Testing

The system supports up to five simultaneous client connections for testing.

The server runs on a static IP and port for communication.

Players interact through a GUI with refresh buttons for real-time updates.

## Known Limitations

The system does not support large-scale scalability or load balancing.

The lobby update mechanism uses a ‘pull’ strategy instead of real-time push notifications.

Only image and text file sharing is supported.

## Authors

This project was developed as part of a university assignment by a team of two/three members.

## License

This project is for educational purposes only.
