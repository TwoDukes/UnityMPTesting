using UnityEngine.Networking;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private GameObject weaponGFX;
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            enabled = false;
        }

        weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName); //Sets gun to weapon layer to be rendered by weapon camera only
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    
    [Client]
    private void Shoot()
    {
        RaycastHit _hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask)) //if we hit something
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string _PlayerID, int _damage)
    {
        Debug.Log(_PlayerID + " has been shot");

        Player _player = GameManager.GetPlayer(_PlayerID);
        _player.RpcTakeDamage(_damage);
    }

}
