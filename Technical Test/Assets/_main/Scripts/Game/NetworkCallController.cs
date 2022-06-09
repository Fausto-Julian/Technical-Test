using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkCallController : MonoBehaviour
{
    private GameManager _gameManager;
    private PhotonView _photonView;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _photonView = GetComponent<PhotonView>();
    }

    public void SendAddPoints(int addPoints, bool playerOne)
    {
        _photonView.RPC(nameof(RPC_SendAddPoints), RpcTarget.All, addPoints, playerOne);
    }
    
    public void SendRemovePoints(int removePoints, bool playerOne)
    {
        _photonView.RPC(nameof(RPC_SendRemovePoints), RpcTarget.All, removePoints, playerOne);
    }

    public void SendNamePlayerOne(string nickName)
    {
        _photonView.RPC(nameof(RPC_SendNamePlayerOne), RpcTarget.All, nickName);
    }
    
    public void SendNamePlayerTwo(string nickName)
    {
        _photonView.RPC(nameof(RPC_SendNamePlayerTwo), RpcTarget.All, nickName);
    }

    [PunRPC]
    private void RPC_SendAddPoints(int addPoints, bool playerOne)
    {
        _gameManager.AddPointsMultiplayer(addPoints, playerOne);
    }
    
    [PunRPC]
    private void RPC_SendRemovePoints(int removePoints, bool playerOne)
    {
        _gameManager.RemovePointsMultiplayer(removePoints, playerOne);
    }
    
    [PunRPC]
    private void RPC_SendNamePlayerOne(string nickName)
    {
        _gameManager.SetPlayerOneName(nickName);
    }
    
    [PunRPC]
    private void RPC_SendNamePlayerTwo(string nickName)
    {
        _gameManager.SetPlayerTwoName(nickName);
    }
}