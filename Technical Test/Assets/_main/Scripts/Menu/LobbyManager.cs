using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login Panel")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private InputField playerNameInput;
    [SerializeField] private Button loginButton;

    [Header("Selection Panel")]
    [SerializeField] private GameObject selectionPanel;
    
    [Header("Create Room Panel")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private Button createRoomPanelButton;
    
    [Header("Join Random Room Panel")]
    [SerializeField] private GameObject joinRandomRoomPanel;
    [SerializeField] private Button randomRoomButton;
    
    [Header("Room List Panel")]
    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private Button roomListPanelButton;
    
    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (GameSettings.NickPlayer == string.Empty)
        {
            playerNameInput.text = "Player " + Random.Range(0, 10000);
        }
        else
        {
            playerNameInput.text = GameSettings.NickPlayer;
        }
    }

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        createRoomPanelButton.onClick.AddListener(OnCreateRoomPanelClicked);
        randomRoomButton.onClick.AddListener(OnJoinRandomRoomButtonClicked);
        roomListPanelButton.onClick.AddListener(OnRoomListButtonClicked);
    }
    
    public override void OnConnectedToMaster()
    {
        SetActivePanel(selectionPanel.name);
    }

    #region Buttons

    private void OnLoginButtonClicked()
    {
        var playerName = playerNameInput.text;

        if (!playerName.Equals(""))
        {
            GameSettings.IsMultiplayer = true;
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
    
    // Go to List Rooms
    private void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(roomListPanel.name);
    }
    
    // Go to Create Rooms
    private void OnCreateRoomPanelClicked()
    {
        SetActivePanel(createRoomPanel.name);
    }
    
    // Join Random Room
    private void OnJoinRandomRoomButtonClicked()
    {
        SetActivePanel(joinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    private void SetActivePanel(string activePanel)
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        selectionPanel.SetActive(activePanel.Equals(selectionPanel.name));
        createRoomPanel.SetActive(activePanel.Equals(createRoomPanel.name));
        joinRandomRoomPanel.SetActive(activePanel.Equals(joinRandomRoomPanel.name));
        roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));
    }

    
}