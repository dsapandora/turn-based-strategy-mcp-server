using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Network;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Controllers;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Network
{
    /// <summary>
    /// An abstract class for managing network connections in a multiplayer game.
    /// Implementations of this class can provide different online backends such as Nakama, Photon, etc.
    /// </summary>
    public abstract class NetworkConnection : MonoBehaviour, INetworkConnection
    {
        [SerializeField] private UnityGridController _gridController;

        /// <summary>
        /// Event triggered when the server connection is successfully established.
        /// </summary>
        public event EventHandler ServerConnected;

        /// <summary>
        /// Event triggered when a room is successfully joined. Carries room data as event arguments.
        /// </summary>
        public event EventHandler<RoomData> RoomJoined;

        /// <summary>
        /// Event triggered when the local player exits a room.
        /// </summary>
        public event EventHandler RoomExited;

        /// <summary>
        /// Event triggered when a new player enters the room. Carries the player's network user data.
        /// </summary>
        public event EventHandler<NetworkUser> PlayerEnteredRoom;

        /// <summary>
        /// Event triggered when a player leaves the room. Carries the player's network user data.
        /// </summary>
        public event EventHandler<NetworkUser> PlayerLeftRoom;

        /// <summary>
        /// Event triggered if joining a room fails. Carries a message detailing the failure.
        /// </summary>
        public event EventHandler<string> JoinRoomFailed;

        /// <summary>
        /// Event triggered if creating a room fails. Carries a message detailing the failure.
        /// </summary>
        public event EventHandler<string> CreateRoomFailed;

        /// <summary>
        /// Property indicating if the local player is the host of the current room.
        /// </summary>
        public virtual bool IsHost { get; protected set; }

        protected Dictionary<long, Action<Dictionary<string, object>>> Handlers = new Dictionary<long, Action<Dictionary<string, object>>>();
        protected Queue<Func<Task>> EventQueue = new Queue<Func<Task>>();
        protected bool processingEvents;

        /// <summary>
        /// Connect to the multiplayer game server.
        /// </summary>
        /// <param name="userName">The name of the user connecting to the server.</param>
        /// <param name="customParams">Additional custom parameters for the connection.</param>
        public abstract void ConnectToServer(string userName, Dictionary<string, string> customParams);

        /// <summary>
        /// Join a quick match with the specified maximum number of players.
        /// </summary>
        /// <param name="maxPlayers">Maximum number of players in the match.</param>
        /// <param name="customParams">Additional custom parameters for the match.</param>
        public abstract void JoinQuickMatch(int maxPlayers, Dictionary<string, string> customParams);

        /// <summary>
        /// Create a new room with the specified parameters.
        /// </summary>
        /// <param name="roomName">Name of the room to create.</param>
        /// <param name="maxPlayers">Maximum number of players in the room.</param>
        /// <param name="isPrivate">Whether the room is private. A private room will not be listed by the <see cref="GetRoomList"/> method</param>
        /// <param name="customParams">Additional custom parameters for the room.</param>
        public abstract void CreateRoom(string roomName, int maxPlayers, bool isPrivate, Dictionary<string, string> customParams);

        /// <summary>
        /// Join an existing room by its name.
        /// </summary>
        /// <param name="roomName">The name of the room to join.</param>
        public abstract void JoinRoomByName(string roomName);

        /// <summary>
        /// Join an existing room by its unique ID.
        /// </summary>
        /// <param name="roomID">The unique identifier of the room to join.</param>
        public abstract void JoinRoomByID(string roomID);

        /// <summary>
        /// Leave the current room.
        /// </summary>
        public abstract void LeaveRoom();

        /// <summary>
        /// Get a list of available public rooms.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of RoomData.</returns>
        public abstract Task<IEnumerable<RoomData>> GetRoomList();

        /// <summary>
        /// Send the current match state to other players in the room.
        /// </summary>
        /// <param name="opCode">Operation code indicating the type of the match state.</param>
        /// <param name="actionParams">Parameters representing the match state.</param>
        public abstract void SendMatchState(long opCode, IDictionary<string, object> actionParams);

        /// <summary>
        /// Adds a handler for processing specific network operations identified by an operation code.
        /// </summary>
        /// <param name="handler">The action to perform when the specified OpCode is received. The action takes a dictionary of string key-value pairs representing the parameters of the network operation.</param>
        /// <param name="OpCode">The operation code that identifies the type of network operation.</param>
        public virtual void AddHandler(Action<Dictionary<string, object>> handler, long OpCode)
        {
            Handlers.Add(OpCode, handler);
        }

        /// <summary>
        /// Initializes the random number generator with a specific seed. This is useful for ensuring that random number generation is consistent and replicable across different instances of the game, which is important in multiplayer environments.
        /// </summary>
        /// <param name="seed">The seed value to initialize the random number generator. Typically, this seed should be synchronized across all clients in a multiplayer game to ensure consistent random number generation.</param>
        public virtual void InitializeRng(int seed)
        {
            UnityEngine.Random.InitState(seed);
        }

        protected virtual void Awake()
        {
            Handlers.Add((long)OpCode.TurnEnded, HandleRemoteTurnEnding);
            Handlers.Add((long)OpCode.AbilityUsed, HandleRemoteAbilityUsed);
        }

        protected virtual void Start()
        {
            _gridController.GameInitialized += OnGameInitialized;
        }

        private void OnGameInitialized()
        {
            foreach (var unit in _gridController.UnitManager.GetUnits())
            {
                unit.AbilityUsed += OnAbilityUsedLocal;
            }
            _gridController.UnitManager.UnitAdded += OnUnitAdded;
            _gridController.TurnEnded += OnTurnEndedLocal;
        }

        protected void InvokeServerConnected()
        {
            Debug.Log("Server connected");
            ServerConnected?.Invoke(this, EventArgs.Empty);
        }
        protected void InvokeRoomJoined(RoomData roomData)
        {
            var players = roomData.Users.ToList();
            Debug.Log($"Joined room: {roomData.RoomID}; players inside: {players.Count}");
            RoomJoined?.Invoke(this, roomData);
        }

        protected void InvokeRoomExited()
        {
            Debug.Log("Exited room");
            RoomExited?.Invoke(this, EventArgs.Empty);
        }

        protected void InvokePlayerEnteredRoom(NetworkUser networkUser)
        {
            Debug.Log($"Player {networkUser.UserID} entered room");
            PlayerEnteredRoom?.Invoke(this, networkUser);
        }

        protected void InvokePlayerLeftRoom(NetworkUser networkUser)
        {
            Debug.Log($"Player {networkUser.UserID} left room");
            PlayerLeftRoom?.Invoke(this, networkUser);
        }
        protected void InvokeCreateRoomFailed(string message)
        {
            CreateRoomFailed?.Invoke(this, message);
        }
        protected void InvokeJoinRoomFailed(string message)
        {
            JoinRoomFailed?.Invoke(this, message);
        }

        private void OnUnitAdded(IUnit unit)
        {
            Debug.Log("unit added network connection");
            unit.AbilityUsed += OnAbilityUsedLocal;
        }

        private static class SerializationKeys
        {
            public const string UnitID = "unit_id";
            public const string CommandType = "command_type";
        }

        private void OnAbilityUsedLocal(AbilityUsedEventArgs eventArgs)
        {
            Debug.Log("ability used local network connection");
            //If Ability was triggered by a remote instance, do nothing
            if (eventArgs.IsNetworkInvoked)
            {
                return;
            }

            // If ability was triggered by the local instance, forward it to other players
            var actionParams = eventArgs.Command.Serialize();
            actionParams.Add(SerializationKeys.CommandType, $"{eventArgs.Command.GetType().FullName}, {eventArgs.Command.GetType().Assembly.FullName}");
            actionParams.Add(SerializationKeys.UnitID, eventArgs.Unit.UnitID.ToString());
            SendMatchState((int)OpCode.AbilityUsed, actionParams);
        }
        private void OnTurnEndedLocal(TurnTransitionParams turnTransitionParams)
        {
            //If turn ending was triggered by a remote instance, do nothing
            if (turnTransitionParams.IsNetworkInvoked)
            {
                return;
            }

            // If turn ending was triggered by the local instance, forward it to other players
            SendMatchState((int)OpCode.TurnEnded, new Dictionary<string, object>());
        }

        private void HandleRemoteAbilityUsed(Dictionary<string, object> actionParams)
        {
            Debug.Log("ability used remote network connection");

            var unit = _gridController.UnitManager.GetUnits().First(u => u.UnitID == int.Parse(actionParams[SerializationKeys.UnitID].ToString()));
            var commandType = Type.GetType(actionParams[SerializationKeys.CommandType].ToString());
            {
                var command = Activator.CreateInstance(commandType) as ICommand;
                var initializedCommand = command.Deserialize(actionParams, _gridController);

                EventQueue.Enqueue(() => unit.ExecuteAbility(initializedCommand, (gC) => Task.CompletedTask, (gC) => Task.CompletedTask, true));
            }
            if (!processingEvents)
            {
                _ = ProcessEvents();
            }
        }
        private void HandleRemoteTurnEnding(Dictionary<string, object> actionParams)
        {
            EventQueue.Enqueue(() => EndTurn(true));
            if (!processingEvents)
            {
                _ = ProcessEvents();
            }
        }

        protected Task EndTurn(bool isNetworkInvoked)
        {
            _gridController.EndTurn(true);
            return Task.CompletedTask;
        }

        protected virtual async Task ProcessEvents()
        {
            processingEvents = true;
            while (EventQueue.Count > 0)
            {
                var action = EventQueue.Dequeue();
                await action();
            }
            processingEvents = false;
        }
    }
}