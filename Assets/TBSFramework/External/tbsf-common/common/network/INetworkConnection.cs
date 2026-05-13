using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.Network
{
    /// <summary>
    /// An interface for managing network connections in a multiplayer game.
    /// Implementations of this interface can provide different online backends such as Nakama, Photon, etc.
    /// </summary>
    public interface INetworkConnection
    {
        /// <summary>
        /// Event triggered when the server connection is successfully established.
        /// </summary>
        event EventHandler ServerConnected;

        /// <summary>
        /// Event triggered when a room is successfully joined. Carries room data as event arguments.
        /// </summary>
        event EventHandler<RoomData> RoomJoined;

        /// <summary>
        /// Event triggered when the local player exits a room.
        /// </summary>
        event EventHandler RoomExited;

        /// <summary>
        /// Event triggered when a new player enters the room. Carries the player's network user data.
        /// </summary>
        event EventHandler<NetworkUser> PlayerEnteredRoom;

        /// <summary>
        /// Event triggered when a player leaves the room. Carries the player's network user data.
        /// </summary>
        event EventHandler<NetworkUser> PlayerLeftRoom;

        /// <summary>
        /// Event triggered if joining a room fails. Carries a message detailing the failure.
        /// </summary>
        event EventHandler<string> JoinRoomFailed;

        /// <summary>
        /// Event triggered if creating a room fails. Carries a message detailing the failure.
        /// </summary>
        event EventHandler<string> CreateRoomFailed;

        /// <summary>
        /// Property indicating if the local player is the host of the current room.
        /// </summary>
        bool IsHost { get; }

        /// <summary>
        /// Connect to the multiplayer game server.
        /// </summary>
        /// <param name="userName">The name of the user connecting to the server.</param>
        /// <param name="customParams">Additional custom parameters for the connection.</param>
        void ConnectToServer(string userName, Dictionary<string, string> customParams);

        /// <summary>
        /// Join a quick match with the specified maximum number of players.
        /// </summary>
        /// <param name="maxPlayers">Maximum number of players in the match.</param>
        /// <param name="customParams">Additional custom parameters for the match.</param>
        void JoinQuickMatch(int maxPlayers, Dictionary<string, string> customParams);

        /// <summary>
        /// Create a new room with the specified parameters.
        /// </summary>
        /// <param name="roomName">Name of the room to create.</param>
        /// <param name="maxPlayers">Maximum number of players in the room.</param>
        /// <param name="isPrivate">Whether the room is private. A private room will not be listed by the <see cref="GetRoomList"/> method</param>
        /// <param name="customParams">Additional custom parameters for the room.</param>
        void CreateRoom(string roomName, int maxPlayers, bool isPrivate, Dictionary<string, string> customParams);

        /// <summary>
        /// Join an existing room by its name.
        /// </summary>
        /// <param name="roomName">The name of the room to join.</param>
        void JoinRoomByName(string roomName);

        /// <summary>
        /// Join an existing room by its unique ID.
        /// </summary>
        /// <param name="roomID">The unique identifier of the room to join.</param>
        void JoinRoomByID(string roomID);

        /// <summary>
        /// Leave the current room.
        /// </summary>
        void LeaveRoom();

        /// <summary>
        /// Get a list of available public rooms.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of RoomData.</returns>
        Task<IEnumerable<RoomData>> GetRoomList();

        /// <summary>
        /// Send the current match state to other players in the room.
        /// </summary>
        /// <param name="opCode">Operation code indicating the type of the match state.</param>
        /// <param name="actionParams">Parameters representing the match state.</param>
        void SendMatchState(long opCode, IDictionary<string, object> actionParams);

        /// <summary>
        /// Adds a handler for processing specific network operations identified by an operation code.
        /// </summary>
        /// <param name="handler">The action to perform when the specified OpCode is received. The action takes a dictionary of string key-value pairs representing the parameters of the network operation.</param>
        /// <param name="OpCode">The operation code that identifies the type of network operation.</param>
        void AddHandler(Action<Dictionary<string, object>> handler, long OpCode);

        /// <summary>
        /// Initializes the random number generator with a specific seed. This is useful for ensuring that random number generation is consistent and replicable across different instances of the game, which is important in multiplayer environments.
        /// </summary>
        /// <param name="seed">The seed value to initialize the random number generator. Typically, this seed should be synchronized across all clients in a multiplayer game to ensure consistent random number generation.</param>
        void InitializeRng(int seed);
    }

    /// <summary>
    /// Represents the data for a room in a multiplayer game.
    /// Contains information about the room, such as the users in the room, room name, and ID.
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// The local user's network user data.
        /// </summary>
        public NetworkUser LocalUser { get; private set; }

        /// <summary>
        /// The collection of network users currently in the room.
        /// </summary>
        public IEnumerable<NetworkUser> Users { get; private set; }

        /// <summary>
        /// The current number of users in the room.
        /// </summary>
        public int UserCount { get; private set; }

        /// <summary>
        /// The maximum number of users allowed in the room.
        /// </summary>
        public int MaxUsers { get; private set; }

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string RoomName { get; private set; }

        /// <summary>
        /// The unique identifier for the room.
        /// </summary>
        public string RoomID { get; private set; }

        /// <summary>
        /// Constructor for creating a new RoomData instance.
        /// </summary>
        /// <param name="localUser">Local user's network data.</param>
        /// <param name="users">List of users in the room.</param>
        /// <param name="userCount">Number of users currently in the room.</param>
        /// <param name="maxUsers">Maximum number of users allowed in the room.</param>
        /// <param name="roomName">Name of the room.</param>
        /// <param name="roomID">Unique identifier of the room.</param>
        public RoomData(NetworkUser localUser, IEnumerable<NetworkUser> users, int userCount, int maxUsers, string roomName, string roomID)
        {
            LocalUser = localUser;
            Users = users;
            UserCount = userCount;
            MaxUsers = maxUsers;
            RoomName = roomName;
            RoomID = roomID;
        }
    }

    /// <summary>
    /// Represents a user in a multiplayer network environment.
    /// Contains information such as the user's ID, name, and custom properties.
    /// </summary>
    public class NetworkUser
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        public string UserID { get; private set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Indicates whether the user is the host of the room.
        /// </summary>
        public bool IsHost { get; private set; }

        /// <summary>
        /// Custom properties associated with the user, represented as key-value pairs.
        /// </summary>
        public Dictionary<string, string> CustomProperties { get; private set; }

        /// <summary>
        /// Constructor for creating a new NetworkUser instance.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userID">Unique identifier of the user.</param>
        /// <param name="customProperties">Custom properties of the user.</param>
        /// <param name="isHost">Indicates whether the user is the host of the room.</param>
        public NetworkUser(string userName, string userID, Dictionary<string, string> customProperties, bool isHost = false)
        {
            UserName = userName;
            UserID = userID;
            CustomProperties = customProperties;
            IsHost = isHost;
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkUser && (obj as NetworkUser).UserID.Equals(UserID);
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }
    }


    public enum OpCode : long
    {
        TurnEnded,
        AbilityUsed,
        PlayerNumberChanged,
        IsReadyChanged,
    }
}

