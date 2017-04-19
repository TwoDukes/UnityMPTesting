using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    private WeaponManager weaponManager;

    private PlayerWeapon currentWeapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();

        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            enabled = false;
        }
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn)
            return;

        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            } else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }
    
    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
            return;

        //We are shooting, call the OnShoot method on the server
        CmdOnShoot();

        RaycastHit _hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)) //if we hit something
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(_hit.point, _hit.normal); //we hit something, call onHit method on server
        }
    }

    //This is called on the server when the player shoots
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }
    
    //This is called on all clients when we need a shoot effect
    [ClientRpc]
    private void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play(); //plays particle effect (muzzle flash)
    }

    //Is called on server when we hit something
    //takes in the hit point and the surface normal
    [Command]
    private void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoImpactEffect(_pos, _normal);
    }

    //Is called on all clients
    //here we can spawn in cool effects
    [ClientRpc]
    private void RpcDoImpactEffect(Vector3 _pos, Vector3 _normal)
    {
        //TODO: should do object pooling later
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().impactEffect, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    //Called on the server when a player is shot
    [Command]
    private void CmdPlayerShot(string _PlayerID, int _damage, string _sourceID)
    {
        Debug.Log(_PlayerID + " has been shot");

        Player _player = GameManager.GetPlayer(_PlayerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }

}
