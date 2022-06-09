
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text playersText;
    [SerializeField] private Text difficultyText;
    [SerializeField] private Button joinRoomButton;

    private string _roomName;

    public void Start()
    {
        joinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(_roomName);
        });
    }

    public void Initialize(string roomName, byte currentPlayers, byte maxPlayers)
    {
        _roomName = roomName;

        nameText.text = roomName;
        playersText.text = currentPlayers + " / " + maxPlayers;
    }
}