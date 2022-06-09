using UnityEngine;

public static class GameSettings
{
    public const string READY = "IsPlayerReady";
    public const string DIFFICULTY_LEVEL = "GameDifficult";
    
    public static byte Difficulty = 0;
    public static bool IsMultiplayer = false;

    public static string NickPlayer = string.Empty;

    public static Color GetColor(int colorChoice)
    {
        return colorChoice switch
        {
            0 => Color.red,
            1 => Color.green,
            2 => Color.blue,
            3 => Color.yellow,
            _ => Color.black
        };
    }
}