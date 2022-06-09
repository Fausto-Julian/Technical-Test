
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatusUI : MonoBehaviour
{
    [SerializeField] private Text connectionText;
    private const string CONNECTION_STATUS_MESSAGE = " Connection Status: ";

    private void Update()
    {
        connectionText.text = CONNECTION_STATUS_MESSAGE + PhotonNetwork.NetworkClientState;
    }
}