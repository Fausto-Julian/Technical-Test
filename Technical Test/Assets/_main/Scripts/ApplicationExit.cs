using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationExit : MonoBehaviour
{
    [SerializeField] private Button exitGameButton;
    private void Start()
    {
        exitGameButton.onClick.AddListener(OnExitGameClicked);
    }

    private void OnExitGameClicked()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        
        Application.Quit();
    }
}
