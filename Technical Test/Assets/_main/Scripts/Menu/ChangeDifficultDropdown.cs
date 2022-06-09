using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficultDropdown : MonoBehaviour
{
    [SerializeField] private Dropdown dropdown;

    private void Awake()
    {
        dropdown.value = 0;
        dropdown.onValueChanged.AddListener(OnChangeValueHandler);
    }

    private void OnChangeValueHandler(int value)
    {
        GameSettings.Difficulty = (byte)value;
    }
}