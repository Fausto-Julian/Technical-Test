using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private PhotonView _photonView;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Start()
    {
        if (GameSettings.IsMultiplayer)
        {
            _photonView = GetComponent<PhotonView>();
        }
    }

    private void Update()
    {
        if (GameSettings.IsMultiplayer)
        {
            if (_photonView.IsMine)
            {
                Move();
                
                Hit();
            }
        }
        else
        {
            Move();
            
            Hit();
        }
    }

    private void Move()
    {
        // Move Position to mouse Position
        
        var newPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void Hit()
    {
        // Hit Targets
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 500f, LayerMask.GetMask("Target")))
            {
                var target = hit.transform.gameObject.GetComponent<ITarget>();
                if (target != null)
                {
                    target.Hit();
                }
            }
        }
    }
}
