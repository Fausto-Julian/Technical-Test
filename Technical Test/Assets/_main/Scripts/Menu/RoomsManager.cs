using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    [Header("Create Room Panel")] 
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button backCreateRoomButton;

    [Header("Room List Panel")] 
    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private GameObject roomListContent;
    [SerializeField] private GameObject roomListPrefab;
    [SerializeField] private Button backRoomListButton;
    
    [Header("Selection Panel")]
    [SerializeField] private GameObject selectionPanel;
    
    [Header("Join Random Room Panel")]
    [SerializeField] private GameObject joinRandomRoomPanel;
    
    [Header("Inside Room Panel")]
    [SerializeField] private GameObject insideRoomPanel;
    [SerializeField] private Button leaveRoomButton;
    
    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, GameObject> _roomList;
    private Dictionary<int, GameObject> _playerList;
    
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject playerListPrefab;

    private void Awake()
    {
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        _roomList = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        backCreateRoomButton.onClick.AddListener(OnBackButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        leaveRoomButton.onClick.AddListener(OnLeaveGameButtonClicked);
        backRoomListButton.onClick.AddListener(OnBackButtonClicked);
    }

    #region Rooms

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomName = "Room " + Random.Range(1000, 10000);

        var props = new Hashtable();
        props.Add(GameSettings.DIFFICULTY_LEVEL, GameSettings.Difficulty);
        
        var options = new RoomOptions {MaxPlayers = 2, CustomRoomProperties = props};

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnJoinedRoom()
    {
        _cachedRoomList.Clear();

        SetActivePanel(insideRoomPanel.name);

        if (_playerList == null)
        {
            _playerList = new Dictionary<int, GameObject>();
        }
        
        // Instance player in current room and set active if is player ready
        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            
            var entry = Instantiate(playerListPrefab, insideRoomPanel.transform, true);

            entry.transform.localScale = Vector3.one;
            
            entry.GetComponent<PlayerListController>().Initialize(player.ActorNumber, player.NickName);

            if (player.CustomProperties.TryGetValue(GameSettings.READY, out var isPlayerReady))
            {
                entry.GetComponent<PlayerListController>().SetPlayerReady((bool)isPlayerReady);
            }

            _playerList.Add(player.ActorNumber, entry);
        }

        startGameButton.gameObject.SetActive(CheckPlayersReady());
        
        // Set local player difficulty equals is room difficulty
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameSettings.DIFFICULTY_LEVEL, out var difficult))
        {
            GameSettings.Difficulty = (byte)difficult;
        }
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(selectionPanel.name);

        foreach (var entry in _playerList.Values)
        {
            Destroy(entry.gameObject);
        }
        
        _playerList.Clear();
        _playerList = null;
    }

    #endregion

    #region Insider Room

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var entry = Instantiate(playerListPrefab, insideRoomPanel.transform, true);

        entry.transform.localScale = Vector3.one;
        
        entry.GetComponent<PlayerListController>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        _playerList.Add(newPlayer.ActorNumber, entry);

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(_playerList[otherPlayer.ActorNumber].gameObject);
        
        _playerList.Remove(otherPlayer.ActorNumber);

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_playerList == null)
        {
            _playerList = new Dictionary<int, GameObject>();
        }

        if (_playerList.TryGetValue(targetPlayer.ActorNumber, out var entry))
        {
            if (changedProps.TryGetValue(GameSettings.READY, out var isPlayerReady))
            {
                entry.GetComponent<PlayerListController>().SetPlayerReady((bool) isPlayerReady);
            }
        }

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #region Buttons
    
    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(selectionPanel.name);
    }
    
    private void OnCreateRoomButtonClicked()
    {
        var roomName = roomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        var props = new Hashtable();
        props.Add(GameSettings.DIFFICULTY_LEVEL, GameSettings.Difficulty);
        
        var options = new RoomOptions {MaxPlayers = 2, PlayerTtl = 10000, CustomRoomProperties = props};

        PhotonNetwork.CreateRoom(roomName, options);
    }

    private void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("GameMultiplayerScene");
    }
    
    private void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Function RoomList

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (var i = 0; i < roomList.Count; i++)
        {
            var roomInfo = roomList[i];
            
            if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(roomInfo.Name))
                {
                    _cachedRoomList.Remove(roomInfo.Name);
                }

                continue;
            }

            if (_cachedRoomList.ContainsKey(roomInfo.Name))
            {
                _cachedRoomList[roomInfo.Name] = roomInfo;
            }
            else
            {
                _cachedRoomList.Add(roomInfo.Name, roomInfo);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (var info in _cachedRoomList.Values)
        {
            var entry = Instantiate(roomListPrefab, roomListContent.transform, true);
            
            entry.transform.localScale = Vector3.one;
            
            //info.CustomProperties.TryGetValue(GameSettings.DIFFICULTY_LEVEL, out var difficulty);
            
            entry.GetComponent<RoomController>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            _roomList.Add(info.Name, entry);
        }
    }
    
    private void ClearRoomListView()
    {
        foreach (var entry in _roomList.Values)
        {
            Destroy(entry.gameObject);
        }

        _roomList.Clear();
    }

    #endregion

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    private void SetActivePanel(string activePanel)
    {
        selectionPanel.SetActive(activePanel.Equals(selectionPanel.name));
        createRoomPanel.SetActive(activePanel.Equals(createRoomPanel.name));
        roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));
        joinRandomRoomPanel.SetActive(activePanel.Equals(joinRandomRoomPanel.name));
        insideRoomPanel.SetActive(activePanel.Equals(insideRoomPanel.name));
    }
    
    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            return false;
        }

        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            if (player.CustomProperties.TryGetValue(GameSettings.READY, out var isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}