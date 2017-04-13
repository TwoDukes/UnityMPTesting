using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerShoot : NetworkBehaviour {

    public playerWeapon weapon;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

	// Use this for initialization
	void Start () {
		if (cam == null)
        {
            Debug.Log("PlayerShoot: No Camera Referenced");
            this.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
	}
    void Shoot ()
    {
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, 
            cam.transform.forward, // Gets the unit vector3 of the z axis of the camera
            out _hit, // C# Output parameter gets assigned the raycast information
            weapon.range, // Maximum distance of the raycast = range of weapon
            mask)) // Layer of items that get hit
        {
            Debug.Log("We hit " + _hit.collider.name);
        }
    }
}
