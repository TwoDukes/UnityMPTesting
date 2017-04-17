using UnityEngine;

[System.Serializable] //makes instances of this classes variables show in inspector
public class PlayerWeapon {

    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;
    public float fireRate = 0f;

    public GameObject graphics;
	
}
