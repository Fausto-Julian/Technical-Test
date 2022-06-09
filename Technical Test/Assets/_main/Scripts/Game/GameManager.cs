using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxPoint = 100;
    [SerializeField] private float maxTime = 120f;

    [Header("Reference Single Player")]
    [SerializeField] private TextMeshProUGUI nickNameSinglePlayerText;
    [SerializeField] private TextMeshProUGUI pointsSinglePlayerText;
    
    [Header("Reference Multiplayer")]
    [SerializeField] private TextMeshProUGUI nickNameMultiPlayerOneText;
    [SerializeField] private TextMeshProUGUI pointsMultiPlayerOneText;
    [SerializeField] private TextMeshProUGUI nickNameMultiPlayerTwoText;
    [SerializeField] private TextMeshProUGUI pointsMultiPlayerTwoText;

    private float _currentTime;
    private int _pointsSinglePlayer;
    private int _pointsPlayerOne;
    private int _pointsPlayerTwo;

    private bool _isFinish;
    private bool _isResult;

    public string PlayerOneNickName { get; private set; }
    public string PlayerTwoNickName { get; private set; }

    public bool IsPlayerOneWin { get; private set; }

    public bool IsPlayerTwoWin { get; private set; }

    private void Awake()
    {
        _currentTime = 0;
        _pointsSinglePlayer = 0;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!GameSettings.IsMultiplayer)
        {
            if (pointsSinglePlayerText != null)
            {
                nickNameSinglePlayerText.text = GameSettings.NickPlayer;
            }
        }
    }

    private void Update()
    {
        if (GameSettings.IsMultiplayer)
        {
            if (!_isFinish)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= maxTime)
                {
                    _isFinish = true;
                    if (_pointsPlayerOne > _pointsPlayerTwo)
                    {
                        IsPlayerOneWin = true;
                        IsPlayerTwoWin = false;
                    }
                    else if (_pointsPlayerTwo > _pointsPlayerOne)
                    {
                        IsPlayerOneWin = false;
                        IsPlayerTwoWin = true;
                    }
                    else
                    {
                        IsPlayerOneWin = false;
                        IsPlayerTwoWin = false;
                    }
                }
                else
                {
                    if (_pointsPlayerOne >= maxPoint)
                    {
                        IsPlayerOneWin = true;
                        IsPlayerTwoWin = false;
                        _isFinish = true;
                    }

                    if (_pointsPlayerTwo >= maxPoint)
                    {
                        IsPlayerOneWin = false;
                        IsPlayerTwoWin = true;
                        _isFinish = true;
                    }
                }
            }
        }
        else
        {
            if (!_isFinish)
            {
                _currentTime += Time.deltaTime;

                if (_currentTime > maxTime && _pointsSinglePlayer < maxPoint)
                {
                    IsPlayerOneWin = false;
                    _isFinish = true;
                }

                if (_pointsSinglePlayer >= maxPoint)
                {
                    IsPlayerOneWin = true;
                    _isFinish = true;
                }
            }
        }

        if (_isFinish && !_isResult)
        {
            _isResult = true;
            GameFinish();
        }
    }

    public void AddPointsSinglePlayer(int addPoints)
    {
        _pointsSinglePlayer += addPoints;
        pointsSinglePlayerText.text = "Points: " + _pointsSinglePlayer;
    }
    
    public void RemovePointsSinglePlayer(int removePoints)
    {
        _pointsSinglePlayer -= removePoints;
        pointsSinglePlayerText.text = "Points: " + _pointsSinglePlayer;
    }
    
    public void AddPointsMultiplayer(int addPoints, bool playerOne)
    {
        if (playerOne)
        {
            _pointsPlayerOne += addPoints;
            pointsMultiPlayerOneText.text = "Points: " + _pointsPlayerOne;
        }
        else
        {
            _pointsPlayerTwo += addPoints;
            pointsMultiPlayerTwoText.text = "Points: " + _pointsPlayerTwo;
        }
    }
    
    public void RemovePointsMultiplayer(int removePoints, bool playerOne)
    {
        if (playerOne)
        {
            _pointsPlayerOne -= removePoints;
            pointsMultiPlayerOneText.text = "Points: " + _pointsPlayerOne;
        }
        else
        {
            _pointsPlayerTwo -= removePoints;
            pointsMultiPlayerTwoText.text = "Points: " + _pointsPlayerTwo;
        }
    }

    public void SetPlayerOneName(string nickName)
    {
        PlayerOneNickName = nickName;
        nickNameMultiPlayerOneText.text = nickName;
    }
    
    public void SetPlayerTwoName(string nickName)
    {
        PlayerTwoNickName = nickName;
        nickNameMultiPlayerTwoText.text = nickName;
    }

    private void GameFinish()
    {
        Cursor.visible = true;
        if (GameSettings.IsMultiplayer)
        {
            PhotonNetwork.LoadLevel("ResultScene");
        }
        else
        {
            SceneManager.LoadScene("ResultScene");
        }
    }
}