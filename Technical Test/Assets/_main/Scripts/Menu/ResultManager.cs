using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button backToMainMenuButton;
    [SerializeField] private Button exitGame;
    
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();

        if (GameSettings.IsMultiplayer)
        {
            if (_gameManager.IsPlayerOneWin)
            {
                title.text = "Victory " + _gameManager.PlayerOneNickName;
            }
            else if (_gameManager.IsPlayerTwoWin)
            {
                title.text = "Victory " + _gameManager.PlayerTwoNickName;
            }
            else
            {
                title.text = "Draw";
            }
        }
        else
        {
            title.text = _gameManager.IsPlayerOneWin ? "Victory" : "Defeat";
        }
        
        backToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);
        exitGame.onClick.AddListener(OnExitGameClicked);
    }

    private void OnBackToMainMenuClicked()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        
        Destroy(_gameManager.gameObject);

        SceneManager.LoadScene("MenuScene");
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