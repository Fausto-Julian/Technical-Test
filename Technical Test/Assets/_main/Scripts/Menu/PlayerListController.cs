using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text nameText;

    [SerializeField] private Image colorImage;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image readyImage;

    private int _ownerId;
    private bool _isPlayerReady;
    
    public void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != _ownerId)
        {
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            var initialProps = new Hashtable {{GameSettings.READY, _isPlayerReady}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);

            readyButton.onClick.AddListener(() =>
            {
                _isPlayerReady = !_isPlayerReady;
                SetPlayerReady(_isPlayerReady);
                
                // Change properties player network
                var props = new Hashtable {{GameSettings.READY, _isPlayerReady}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                
                // Update Player Properties Room
                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<RoomsManager>().LocalPlayerPropertiesUpdated();
                }
            });
        }
    }

    public void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }

    public void Initialize(int playerId, string playerName)
    {
        _ownerId = playerId;
        nameText.text = playerName;
    }

    private void OnPlayerNumberingChanged()
    {
        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            if (player.ActorNumber == _ownerId)
            {
                colorImage.color = GameSettings.GetColor(player.GetPlayerNumber());
            }
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        readyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
        readyImage.enabled = playerReady;
    }
}