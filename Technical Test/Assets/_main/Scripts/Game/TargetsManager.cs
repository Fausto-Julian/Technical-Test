using System;
using System.Linq;
using GameEngine;
using GameEngine.Game;
using Photon.Pun;
using UnityEngine;

[System.Serializable]
public struct ScreenEnds
{
    public float min;
    public float max;
}
public class TargetsManager : MonoBehaviour
{
    [Header("Settings Respawn")]
    [SerializeField] private ScreenEnds widthEnds;
    [SerializeField] private ScreenEnds heightEnds;
    [SerializeField] private bool playerOne;

    [SerializeField] private SettingsSpawnerSo dataSettings;
    
    [Header("Reference")]
    [SerializeField] private GameObject[] targetsPrefabs;
    
    private NetworkCallController _networkCallController;
    private GameManager _gameManager;
    
    private float _currentTime;
    
    private readonly PoolGeneric<TargetController> _poolTargets = new PoolGeneric<TargetController>();

    private bool _nextTargetCoin;
    private int _countNextTargetCoin;

    private int _contTargets;
    
    private void Start()
    {
        targetsPrefabs = targetsPrefabs.OrderBy(x => (int)x.GetComponent<TargetController>().Data.TargetType).ToArray();

        _networkCallController = FindObjectOfType<NetworkCallController>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        // Verify that the elements on the screen are the maximum, if it is not passed, it allows spawning another
        if (_contTargets < dataSettings.MaxElements)
        {
            if (_currentTime >= dataSettings.TimeToSpawn)
            {
                _currentTime = 0;
                if (GameSettings.IsMultiplayer)
                {
                    if (_contTargets <= 5)
                    {
                        var target = CreateTarget();
                        target.Initialize(widthEnds, heightEnds);
                    }
                }
                else
                {
                    if (_contTargets <= 5)
                    {
                        var target = CreateTarget();
                        target.Initialize(widthEnds, heightEnds);
                    }
                }
            }
        }

        // Verify that the elements on the screen are the minimum, if one is missing it will spawn it
        if (_contTargets < dataSettings.MinElements)
        {
            var restElements = dataSettings.MinElements - _contTargets;

            for (var i = 0; i < restElements; i++)
            {
                if (GameSettings.IsMultiplayer)
                {
                    var target = CreateTarget();
                    target.Initialize(widthEnds, heightEnds);
                }
                else
                {
                    if (_contTargets <= 5)
                    {
                        var target = CreateTarget();
                        target.Initialize(widthEnds, heightEnds);
                    }
                }
            }
        }
    }
    
    private TargetController CreateTarget()
    {
        var guid = Guid.NewGuid();
        var justNumbers = new string(guid.ToString().Where(char.IsDigit).ToArray());
        var seed = int.Parse(justNumbers.Substring(0, 4));
            
        var random = new System.Random(seed);
        
        // Generate random index for objects
        var index = random.Next(0, 6);
        
        // Generate Coins if an object is hit
        if (_nextTargetCoin)
        {
            index = (int)TargetType.Coin;
            _countNextTargetCoin--;
            if (_countNextTargetCoin <= 0)
            {
                _nextTargetCoin = false;
            }
        }
        
        // Request an object from the pool
        var target = _poolTargets.GetorCreate(index);

        // Create new object if value is null
        if (target.Value == null)
        {
            if (GameSettings.IsMultiplayer)
            {
                // Generate new pos random
                var newX = (float)(random.NextDouble() * (widthEnds.max - widthEnds.min) + widthEnds.min);
                var newY = (float)(random.NextDouble() * (heightEnds.max - heightEnds.min) + heightEnds.min);
                
                target.Value = PhotonNetwork.Instantiate(targetsPrefabs[index].name, new Vector3(newX, newY, targetsPrefabs[index].transform.position.z), targetsPrefabs[index].transform.rotation).GetComponent<TargetController>();
            }
            else
            {
                target.Value = Instantiate(targetsPrefabs[index], targetsPrefabs[index].transform.position, targetsPrefabs[index].transform.rotation).GetComponent<TargetController>();
                
                // Disable online component
                target.Value.GetComponent<PhotonTransformViewClassic>().enabled = false;
                target.Value.GetComponent<PhotonView>().enabled = false;
            }
            
            target.Value.OnDeactivate += ((isHit) =>
            {
                // Check if the target removes points or adds points
                if (GameSettings.IsMultiplayer)
                {
                    ChangePointsOnline(target.Value.Data, isHit);
                }
                else
                {
                    ChangePointsOffline(target.Value.Data, isHit);
                }
                
                //Deactivate Target
                target.Value.gameObject.transform.position = new Vector3(500f, 500f, target.Value.gameObject.transform.position.z);

                _poolTargets.InUseToAvailable(target);
                
                _contTargets--;
            } );
        }
        _contTargets++;
        
        return target.Value;
    }

    private void ChangePointsOnline(TargetDataSo data, bool isHit)
    {
        switch (data.TargetType)
        {
            case TargetType.Coin:
            case TargetType.BlueSphere:
            case TargetType.YellowBlock:
            case TargetType.Shield:
                if (isHit)
                {
                    _networkCallController.SendAddPoints(data.PointsAdd, playerOne);
                }
                else
                {
                    _networkCallController.SendRemovePoints(data.PointsRemove, playerOne);
                }
                break;
            case TargetType.RedBlock:
                if (isHit)
                {
                    _networkCallController.SendRemovePoints(data.PointsRemove, playerOne);
                }
                break;
            case TargetType.Object:
                if (isHit)
                {
                    _countNextTargetCoin = 3;
                    _nextTargetCoin = true;
                }
                else
                {
                    _networkCallController.SendRemovePoints(data.PointsRemove, playerOne);
                }
                break;
        }
    }

    private void ChangePointsOffline(TargetDataSo data, bool isHit)
    {
        switch (data.TargetType)
        {
            case TargetType.Coin:
            case TargetType.BlueSphere:
            case TargetType.YellowBlock:
            case TargetType.Shield:
                if (isHit)
                {
                    _gameManager.AddPointsSinglePlayer(data.PointsAdd);
                }
                else
                {
                    _gameManager.RemovePointsSinglePlayer(data.PointsRemove);
                }
                break;
            case TargetType.RedBlock:
                if (isHit)
                {
                    _gameManager.RemovePointsSinglePlayer(data.PointsRemove);
                }
                break;
            case TargetType.Object:
                if (isHit)
                {
                    _countNextTargetCoin = 3;
                    _nextTargetCoin = true;
                }
                else
                {
                    _gameManager.RemovePointsSinglePlayer(data.PointsRemove);
                }
                break;
        }
    }
}