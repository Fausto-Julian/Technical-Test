
using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public enum TargetType
{
    BlueSphere,
    Coin,
    Shield,
    Object,
    YellowBlock,
    RedBlock
}

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformViewClassic))]
public class TargetController : MonoBehaviour, ITarget
{
    [SerializeField] private TargetDataSo data;
    public TargetDataSo Data => data;

    public event Action<bool> OnDeactivate;

    private int _currentClicks;
    private bool _isHit;

    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void Initialize(ScreenEnds widthEnds, ScreenEnds heightEnds)
    {
        // Generate seed random
        var guid = Guid.NewGuid();
        var justNumbers = new string(guid.ToString().Where(char.IsDigit).ToArray());
        var seed = int.Parse(justNumbers.Substring(0, 4));
        
        // Create class System Random with my own seed
        var random = new Random(seed);

        // Generate new pos random
        var newX = (float)(random.NextDouble() * (widthEnds.max - widthEnds.min) + widthEnds.min);
        var newY = (float)(random.NextDouble() * (heightEnds.max - heightEnds.min) + heightEnds.min);
        
        transform.position = new Vector3(newX, newY, transform.position.z);
        
        _isHit = false;
        
        if (GameSettings.IsMultiplayer)
        {
            if (_photonView == null)
            {
                _photonView = GetComponent<PhotonView>();
            }
            
            if (_photonView.IsMine)
            {
                StartCoroutine(nameof(Deactivate));
            }
        }
        else
        {
            StartCoroutine(nameof(Deactivate));
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(data.TimeIsAlive);
        _currentClicks = 0;
        _isHit = false;
        OnDeactivate.Invoke(_isHit);
    }

    public void Hit()
    {
        // Add hit count
        _currentClicks++;
        
        // Check if the clicks are the ones needed to destroy
        if (_currentClicks >= data.NumberClicksToDestroy)
        {
            if (GameSettings.IsMultiplayer)
            {
                if (_photonView.IsMine)
                {
                    _currentClicks = 0;
                    
                    StopCoroutine(nameof(Deactivate));
                    
                    _isHit = true;
                    OnDeactivate.Invoke(_isHit);
                    
                }
            }
            else
            {
                _currentClicks = 0;
                
                StopCoroutine(nameof(Deactivate));
                
                _isHit = true;
                OnDeactivate.Invoke(_isHit);
            }
            
        }
    }
}