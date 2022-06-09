using UnityEngine;

namespace GameEngine.Game
{
    [CreateAssetMenu(fileName = "SettingsSpawnerSo", menuName = "SettingsSpawner", order = 0)]
    public class SettingsSpawnerSo : ScriptableObject
    {
        [Header("Easy")]
        [SerializeField] private int countMinElementsEasy;
        [SerializeField] private int countMaxElementsEasy;
        [SerializeField] private float timeToSpawnEasy;
    
        [Header("Normal")]
        [SerializeField] private int countMinElementsNormal;
        [SerializeField] private int countMaxElementsNormal;
        [SerializeField] private float timeToSpawnNormal;
    
        [Header("Hard")]
        [SerializeField] private int countMinElementsHard; 
        [SerializeField] private int countMaxElementsHard;
        [SerializeField] private float timeToSpawnHard;

        public int MinElements
        {
            get
            {
                return GameSettings.Difficulty switch
                {
                    0 => countMinElementsEasy,
                    1 => countMinElementsNormal,
                    2 => countMinElementsHard,
                    _ => countMinElementsEasy
                };
            }
        }
        public int MaxElements
        {
            get
            {
                return GameSettings.Difficulty switch
                {
                    0 => countMaxElementsEasy,
                    1 => countMaxElementsNormal,
                    2 => countMaxElementsHard,
                    _ => countMaxElementsEasy
                };
            }
        }
        
        public float TimeToSpawn
        {
            get
            {
                return GameSettings.Difficulty switch
                {
                    0 => timeToSpawnEasy,
                    1 => timeToSpawnNormal,
                    2 => timeToSpawnHard,
                    _ => timeToSpawnEasy
                };
            }
        }
    }
}