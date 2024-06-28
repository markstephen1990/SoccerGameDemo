using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Quantum.Demo;
using Quantum;
using TMPro;
using System.Linq;

public unsafe class UIMainMenu : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks
{
    public int maxPlayers = 4;
    public RuntimeConfigContainer RuntimeConfigContainer;
    public ClientIdProvider.Type IdProvider = ClientIdProvider.Type.NewGuid;
    private AssetGuid selectedMapGuid;
    public TMP_Text winsText;
    public GameObject MainMenu;

    private AppSettings appSettings;
    private string deviceId = "";
    private LoadBalancingClient loadBalancingClient;
    private List<AssetGuid> _mapGuids;
    private bool isGameStarted = false;

    public enum PhotonEventCode : byte
    {
        StartGame = 110
    }

    private void Start()
    {
        var maps = Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);

        _mapGuids = maps.Select(m => m.AssetObject.Guid).ToList();

        if (RuntimeConfigContainer.Config.Map.Id.IsValid == false && _mapGuids.Count == 1)
        {
            selectedMapGuid = _mapGuids[0];
        }
        else
        {
            selectedMapGuid = RuntimeConfigContainer.Config.Map.Id;
        }

        loadBalancingClient = new LoadBalancingClient();
        appSettings = PhotonServerSettings.Instance.AppSettings;
        deviceId = SystemInfo.deviceUniqueIdentifier;
        //selectedMapGuid = RuntimeConfigContainer.Config.Map.Id;
        QuantumEvent.Subscribe<EventGameplayEnded>(this, OnGameEnd);
        UpdateWins();
    }

    private void Update()
    {
        loadBalancingClient?.Service(); // Ensure LoadBalancingClient is serviced if it exists

        // Check if client is in a room
        if (loadBalancingClient != null && loadBalancingClient.InRoom)
        {
            // Check if game has started based on custom room properties
            var hasStarted = loadBalancingClient.CurrentRoom.CustomProperties.TryGetValue("START", out var start) && (bool)start;
            var mapGuid = (AssetGuid)(loadBalancingClient.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out var guid) ? (long)guid : 0L);

            // Master client logic to set map and start game if necessary
            if (loadBalancingClient.LocalPlayer.IsMasterClient)
            {
                var ht = new Hashtable();

                // Set map and start properties if not already set
                if (!mapGuid.IsValid)
                {
                    if (selectedMapGuid.IsValid)
                    {
                        ht.Add("MAP-GUID", selectedMapGuid.Value);
                        ht.Add("START", true);
                    }
                }

                // Apply changes to room properties if any updates were made
                if (ht.Count > 0)
                {
                    loadBalancingClient.CurrentRoom.SetCustomProperties(ht);
                }
            }

            // Start the game if map and start properties are valid and game has not started yet
            if (mapGuid.IsValid && !isGameStarted)
            {
                Debug.LogFormat("### Starting game using map '{0}'", mapGuid);

                // Create runtime configuration based on selected map
                var config = RuntimeConfigContainer != null ? RuntimeConfig.FromByteArray(RuntimeConfig.ToByteArray(RuntimeConfigContainer.Config)) : new RuntimeConfig();
                config.Map.Id = mapGuid;

                // Set parameters for starting the game
                var param = new QuantumRunner.StartParameters
                {
                    RuntimeConfig = config,
                    DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                    GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
                    PlayerCount = loadBalancingClient.CurrentRoom.MaxPlayers,
                    LocalPlayerCount = 1,
                    NetworkClient = loadBalancingClient
                };

                // Create client ID and start the game
                var clientId = ClientIdProvider.CreateClientId(IdProvider, loadBalancingClient);
                QuantumRunner.StartGame(clientId, param);

                // Update game start status and deactivate main menu UI
                isGameStarted = true;
                MainMenu.SetActive(false);
            }
        }
    }


    private void OnDestroy()
    {
        QuantumEvent.UnsubscribeListener(this);
        if (loadBalancingClient != null && loadBalancingClient.IsConnected == true)
        {
            loadBalancingClient.Disconnect();
        }
    }
    public static int numberOfWins
    {
        get => PlayerPrefs.GetInt("Wins");
        set => PlayerPrefs.SetInt("Wins", value);
    }

    public void ConnectToServer()
    {
        loadBalancingClient = new LoadBalancingClient();
        loadBalancingClient.ConnectionCallbackTargets.Add(this);
        loadBalancingClient.MatchMakingCallbackTargets.Add(this);
        loadBalancingClient.AppId = appSettings.AppIdRealtime;
        loadBalancingClient.AppVersion = appSettings.AppVersion;
        loadBalancingClient.ConnectToRegionMaster(appSettings.FixedRegion);
    }

    private void UpdateWins()
    {
        winsText.text = "Wins:" + numberOfWins;
    }

    private void OnGameEnd(EventGameplayEnded e)
    {
        CheckUpdatePlayerWin();
        loadBalancingClient.Disconnect();
    }

    private void CheckUpdatePlayerWin()
    {
        var f = QuantumRunner.Default.Game.Frames.Verified;

        for (int i = 0; i < Constants.MAX_PLAYERS; i++)
        {
            if (f.Global->Players[i].ControlledCharacter != EntityRef.None)
            {
                PlayerLink playerLink = f.Get<PlayerLink>(f.Global->Players[i].ControlledCharacter);
                if (QuantumRunner.Default.Game.PlayerIsLocal(playerLink.Player) && f.Global->Players[i].PlayerScore >= 3)
                {
                    numberOfWins++;
                    UpdateWins();
                    return;
                }
            }
        }
    }



    public void OnConnected()
    {
        Debug.Log("<===========================>OnConnected<===========================>");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("<===========================>OnConnectedToMaster<===========================>");
        loadBalancingClient.OpJoinRandomRoom(new OpJoinRandomRoomParams { MatchingType = MatchmakingMode.FillRoom});
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("<===========================>OnDisconnected<===========================>");
        QuantumRunner.ShutdownAll(true);
        isGameStarted = false;
        MainMenu.SetActive(true);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("<===========================>OnJoinRandomFailed<===========================>");
        RoomOptions roomOptions = new RoomOptions
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayers,
            Plugins = new string[] { "QuantumPlugin" }
        };

        loadBalancingClient.OpCreateRoom(new EnterRoomParams() { RoomOptions = roomOptions });

        Debug.LogFormat("Creating new room for '{0}' max players", maxPlayers);
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("<===========================>OnRegionListReceived<===========================>");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log("<===========================>OnCustomAuthenticationResponse<===========================>");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("<===========================>OnCustomAuthenticationFailed<===========================>");
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        Debug.Log("<===========================>OnFriendListUpdate<===========================>");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("<===========================>OnCreatedRoom<===========================>");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<===========================>OnCreateRoomFailed<===========================>");
    }

    public void OnJoinedRoom()
    {
        Debug.Log("<===========================>OnJoinedRoom<===========================>");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("<===========================>OnJoinRoomFailed<===========================>");
    }

    public void OnLeftRoom()
    {
        Debug.Log("<===========================>OnLeftRoom<===========================>");
    }
}
