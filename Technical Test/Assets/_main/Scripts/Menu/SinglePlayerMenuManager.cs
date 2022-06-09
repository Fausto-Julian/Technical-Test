using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerMenuManager : MonoBehaviour
{
    [SerializeField] private InputField playerNameInput;
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        playerNameInput.onValueChanged.AddListener(OnChangeValueInputFieldHandler);
        playerNameInput.text = string.Empty;
    }

    private void OnStartClicked()
    {
        if (!playerNameInput.text.Equals(""))
        {
            SceneManager.LoadScene("GameSinglePlayerScene");
            GameSettings.IsMultiplayer = false;
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    private void OnChangeValueInputFieldHandler(string nickName)
    {
        GameSettings.NickPlayer = nickName;
    }
}