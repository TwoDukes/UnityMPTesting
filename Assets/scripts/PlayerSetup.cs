using UnityEngine.Networking;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDiable;

    Camera sceneCamera;

    private void Start()
    {
        if(!isLocalPlayer) //if not local player
        {
            foreach(Behaviour item in componentsToDiable) //disable all player control scripts
            {
                item.enabled = false; 
            }
        }
        else //if local player
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false); // turn off lobby camera
            }
        }
            
    }

    private void OnDisable() //basically a destructor
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true); //turns on lobby camera
        }
    }

}
