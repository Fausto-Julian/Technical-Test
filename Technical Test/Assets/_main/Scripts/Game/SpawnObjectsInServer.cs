using Photon.Pun;
using UnityEngine;

public class SpawnObjectsInServer : MonoBehaviour
{
    [SerializeField] private GameObject playerOnePrefab;
    [SerializeField] private GameObject playerTwoPrefab;
    [SerializeField] private GameObject spawnOnePrefab;
    [SerializeField] private GameObject spawnTwoPrefab;

    private NetworkCallController _networkCallController;
    
    private void Start()
    {
        _networkCallController = FindObjectOfType<NetworkCallController>();
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(playerOnePrefab.name, playerOnePrefab.transform.position, playerOnePrefab.transform.rotation);
            Instantiate(spawnOnePrefab);
            _networkCallController.SendNamePlayerOne(PhotonNetwork.NickName);
        }
        else
        {
            PhotonNetwork.Instantiate(playerTwoPrefab.name, playerTwoPrefab.transform.position, playerTwoPrefab.transform.rotation);
            Instantiate(spawnTwoPrefab);
            
            _networkCallController.SendNamePlayerTwo(PhotonNetwork.NickName);
        }
    }
    
    
}